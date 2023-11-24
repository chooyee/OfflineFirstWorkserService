using Factory;
using Factory.DB;
using Factory.Keycloak;
using Factory.Keycloak.Model;
using Model;
using Factory.DB.Model;
using Serilog;
using System.Diagnostics;
using System.Security;
using System.Runtime.Versioning;

namespace Service
{
    [SupportedOSPlatform("windows")]
    internal class OlifAuthService
    {

        public OlifAuthService() { }

        /// <summary>
        /// Login to Local Windows and SSO
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns>LoginResultModel</returns>
        /// <exception cref="Exception"></exception>
        internal async Task<Tuple<bool, string, LoginResultModel>> Login(string username, SecureString password, string domain)
        {  
            //Create return result model
            var loginSession = new LoginResultModel(username, domain);
            bool authResult;

            try
            {
                #region Login to Windows

                // login to Windows 
                // Generate SessionId
                var session = CipherService.GetSessionId(loginSession, username, password, domain);
                authResult = session.Item1;
                if (session.Item1)
                {
                    loginSession.WinAuthStatus = LoginStatus.OfflineAuthActive.ToString();
                    loginSession.SessionId = session.Item2;
                }
                else {
                    loginSession.WinAuthStatus = LoginStatus.AuthFailed.ToString();
                    throw new Exception(session.Item2);
                }
                #endregion

                #region Get Cipherservice
                var cipherService = CipherService.GetCipherService(loginSession);
                #endregion

                #region Login to SSO to get refresh token                
                var ssoLoginResult = await SSOAuthentication(cipherService, loginSession, password);
                if (!ssoLoginResult.Item1)
                {
                    loginSession.SSOAuthStatus = LoginStatus.AuthFailed.ToString();

                    //To allow user working offline event SSO login failed
                    //await loginSession.Save();
                    //return Tuple.Create(false, "Invalid username or password!", loginSession);
                }
                #endregion

                #region Register device id if current fingerprint is empty and sso login = true;
                if (ssoLoginResult.Item1)
                {                    
                    await RegisterDeviceId(cipherService);
                }
                #endregion

                #region Compare device fingerprint agains registered device fingerprint
                var checkRegisteredDevice = await IsRegisteredDevice(cipherService);
                if (!checkRegisteredDevice.Item1)
                {
                    loginSession.DeviceIdCheck = false;
                    throw new Exception(checkRegisteredDevice.Item2);
                }
                #endregion

                #region return success
                loginSession.DeviceIdCheck = true;
                await ClearSessions(loginSession);
                await loginSession.Save();

                return Tuple.Create(authResult, "success", loginSession);
                #endregion
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);

                Log.Error("{funcName}: {error}", funcName, ex.Message);
                await ClearSessions(loginSession);
                await loginSession.Save();
                
                return Tuple.Create(false, ex.Message, loginSession);
            }
        }

        internal async Task<Tuple<bool, string, LoginResultModel>> SSOLogin(string sid, string clientAccessToken)
        {
            //Create return result model
            var loginSession = new LoginResultModel(sid);

            try
            {
                #region Introspect Token
                var jwtToken = await new RHSSOService().Introspect(clientAccessToken);
                //jwtToken.active = true;
                //jwtToken.client_id = "projectwms";
                if (jwtToken.active)
                {

                    if (await GetTrustedClient(jwtToken.client_id))
                    {
                        if (!await loginSession.Load())
                        {
                            throw new Exception("Login session expired!");
                        }
                        Log.Debug($"Username {loginSession.UserName} login via {jwtToken.client_id}");
                    }
                    else
                    {
                        throw new Exception("Untrusted client!");
                    }
                }
                else {
                    throw new Exception("JWT token expired!");
                }
                #endregion

               
                return Tuple.Create(true, "success", loginSession);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);

                Log.Error("{funcName}: {error}", funcName, ex.Message);
                await loginSession.Save();
                return Tuple.Create(false, ex.Message, loginSession);
            }
        }

        /// <summary>
        /// SSOLogin - When successfull authenticate, Save refresh token to DB, set SSOAuthStatus = Active
        /// </summary>
        /// <param name="cipherService"></param>
        /// <param name="loginSession"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static async Task<Tuple<bool, string, SSOToken?>> SSOAuthentication(CipherService cipherService, LoginResultModel loginSession,  SecureString password)
        {
            Log.Debug("SSOLogin for {username}", loginSession.UserName);

            var ssoService = new RHSSOService();

            try
            {
                var ssoHealth = await ssoService.HealthCheck();
                loginSession.SSOHealthStatus = ssoHealth.Item1;

                ///Login to SSO if available 
                if (loginSession.SSOHealthStatus)
                {
                    var ssoToken = await ssoService.GetUserToken(loginSession.UserName, password);
                    
                    Log.Debug("SSO  login successful!");
                    
                    loginSession.SSOAuthStatus = LoginStatus.SSOAuthActive.ToString() ;                    

                    // Save refresh token to local DB
                    Log.Debug("Save refresh token to local DB");
                    var tokenExpiryDate = DateTime.Now.AddSeconds(ssoToken.RefreshExpiresIn);

                    // Encrypt refresh token using AES
                    var enStrResult = cipherService.EncryptLongString(ssoToken.RefreshTokenSecureString().ToCString());

                    // Combine Key and IV into single field
                    var etkiv = $"{enStrResult.EncryptedKeyBase64Str}|{enStrResult.EncryptedKeyBase64Str}";
                    
                    //Save to local DB
                    var userSSO = new ModTableSSOUser(loginSession.UserName, etkiv, enStrResult.EncryptedBase64Str, tokenExpiryDate);
                    await userSSO.Save();
                    //end save

                    return Tuple.Create<bool, string, SSOToken?>(true, "success", ssoToken);
                }
                else
                {
                    Log.Debug("SSO service not available!");
                    return Tuple.Create<bool, string, SSOToken?>(false, "SSO service not available", null);
                }
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                return Tuple.Create<bool, string, SSOToken?>(false, ex.Message, null);
            }
        }
               
        internal static bool WindowsAuthentication(string username, SecureString password, string domain)
        {
            using var winAuth = new WinAuth();
            return winAuth.Auth(username, domain, password);
        }

        /// <summary>
        /// Get SSO Token from local DB
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal async Task<Tuple<SecureString, long>> GetAccessToken(LoginResultModel loginSession)
        {
            try
            {
                var cipherService = CipherService.GetCipherService(loginSession);

                var userSSO = new ModTableSSOUser(loginSession.UserName);
                if (await userSSO.Load())
                {
                    
                    if (DateTime.Now >= DateTime.Parse(userSSO.RefreshTokenExpireDate))
                    {
                        throw new Exception("Refresh token expired! Please connect to VPN and login again");
                    }

                    #region Decrypt Refresh Token
                    //Split AES Key and IV
                    var etkiv = userSSO.ETKiv.Split("|");
                    var encryptedRefreshToken = new EncryptResultModel()
                    {
                        EncryptedKeyBase64Str = etkiv[0],
                        EncryptedIVBase64Str = etkiv[1],
                        EncryptedBase64Str = userSSO.EncryptedRefreshToken
                    };

                    #endregion

                    var ssoService = new RHSSOService();
                    var ssoToken = await ssoService.GetUserToken(cipherService.DecryptLongString(encryptedRefreshToken));
                    return Tuple.Create(ssoToken.AccessTokenSecureString(), ssoToken.ExpiresIn);
                }
                else
                {
                    throw new Exception($"No token found for {loginSession.UserName}! Please connect to VPN and login again");
                }
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        internal async Task<Tuple<bool,string>> IsRegisteredDevice(CipherService cipherService)
        {
            var deviceId = Fingerprint.GenFingerprint();
            var machineLog = await GetRegisteredDeviceId();

            var registeredDeviceId = cipherService.Decrypt(machineLog.Fingerprint);

            if (!deviceId.Equals(registeredDeviceId))
            {
                return Tuple.Create(false, "Device id not match with registered profile!");
            }
            else
            {
                return Tuple.Create(true,"success");
            }
        }

        /// <summary>
        /// GetRegisteredDeviceId
        /// </summary>
        /// <param name="cipherService"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<ModTableMachineLog> GetRegisteredDeviceId()
        {            
            try
            { 
                using var dbContext = new DBContext();

                var tableName = ReflectionFactory.GetTableAttribute(typeof(ModTableMachineLog));
                var query = "select * from " + tableName;
                var result = (await dbContext.ReadMapperAsync<ModTableMachineLog>(query)).First();
                
                return result;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// RegisterDeviceId - register device fingerprint if current table is empty
        /// </summary>
        /// <param name="cipherService"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task RegisterDeviceId(CipherService cipherService)
        {
            try
            {
                var tableName = ReflectionFactory.GetTableAttribute(typeof(ModTableMachineLog));
                using var dbContext = new Factory.DB.DBContext();
                var countResult = await dbContext.ExecuteScalarAsync("select count(*) from " + tableName);
                if (int.Parse(countResult.ToString()) < 1)
                {

                    var deviceId = cipherService.Encrypt(Fingerprint.GenFingerprint());
                    var machineLog = new ModTableMachineLog(deviceId);

                    var result = dbContext.QueryFactory.Insert(machineLog);
                    await dbContext.ExecuteNonQueryAsync(result.Item1, result.Item2);
                }
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }

        }

        private async Task<bool> GetTrustedClient(string clientId)
        {
            try
            {
                TrustedClient client = new TrustedClient()
                {
                    ClientId = clientId
                };

                return await client.Load();
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<int> ClearSessions(LoginResultModel loginSession)
        {
            try
            {
                var table = ReflectionFactory.GetTableAttribute(typeof(LoginResultModel));
                var query = $"Update {table} set sessionId='' where sid<>@sid";
                var sqlParams = new DynamicSqlParameter();
                sqlParams.Add("@sid", loginSession.Sid);

                var dbContext = new DBContext();
                return await dbContext.ExecuteNonQueryAsync(query, sqlParams);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                return 0; //do not throw error!
            }
        }
    }
}
