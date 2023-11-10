using Factory;
using Factory.DB;
using Factory.RHSSOService;
using Factory.RHSSOService.Model;
using Model;
using Factory.DB.Model;
using Serilog;
using System.Diagnostics;
using System.Security;

namespace Service
{
    public class OlifAuthService
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
        public async Task<Tuple<bool,LoginResultModel>> Login(string username, SecureString password, string domain)
        {  
            //Create return result model
            var loginResult = new LoginResultModel(username, domain);
            var authResult = false;
            try
            {
                #region Compare device fingerprint agains registered device fingerprint
                var deviceId = Fingerprint.GenFingerprint();
                var registeredDeviceId = await GetRegisteredDeviceId();

                if (!deviceId.Equals(registeredDeviceId))
                {
                    throw new Exception("Device id not match with registered profile!");
                }
                #endregion

                #region Login to SSO / Windows
                var ssoHealth = await RHSSOLib.HealthCheck();
                loginResult.SSOHealthStatus = ssoHealth.Item1;

                ///Login to SSO if available 
                if (loginResult.SSOHealthStatus)
                {
                    var ssoLoginResult = await SSOLogin(username, password, domain);
                    authResult = ssoLoginResult.Item1;
                    if (authResult)
                    {
                        loginResult.LoginStatus = LoginStatus.SSOAuthActive;
                        loginResult.Token = ssoLoginResult.Item2;
                    }
                    else
                    {
                        loginResult.LoginStatus = LoginStatus.AuthFailed;

                        //Proceed to win auth?
                    }
                }
                else {
                    // login to Windows if SSO not available
                    authResult = new WinAuth().Auth(username, domain, password);
                                       
                    //If Win auth successful, Get SSO refresh token from DB
                    if (authResult)
                    {
                        loginResult.LoginStatus = LoginStatus.OfflineAuthActive;
                        //loginResult.Token = await GetSSOTokenFromLocalDB(username);
                    }

                }
                #endregion
                                
                return Tuple.Create(authResult, loginResult);
                
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);

                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<Tuple<bool, SSOToken?>> SSOLogin(string username, SecureString password, string domain)
        {
            try
            {
                var ssoToken = await RHSSOLib.GetUserToken(GlobalEnv.Instance.UserClient.Client_id, username, password);
                await SaveSSOToken(ssoToken, username);
                return Tuple.Create<bool, SSOToken?>(true, ssoToken);
                
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                return Tuple.Create<bool, SSOToken?>(false, null);
            }
        }

        private async Task<int> SaveSSOToken(SSOToken ssoToken, string username)
        {
            try
            {
                var tokenExpiryDate = DateTime.Now.AddSeconds(ssoToken.RefreshExpiresIn);
                var userSSO = new ModTableSSOUser(username, Factory.Crypto.Cipher.Instance.EncryptString(ssoToken.RefreshTokenSecureString().ToCString()), tokenExpiryDate);
                return await userSSO.Save();
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get SSO Token from local DB
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<SSOToken> GetSSOTokenFromLocalDB(string username)
        {
            try
            {                
                var userSSO = new ModTableSSOUser(username);
                if (await userSSO.Load())
                {
                    var tokenExpireDate = DateTime.Parse(userSSO.RefreshTokenExpireDate);
                    if (DateTime.Now <= tokenExpireDate)
                    {
                        throw new Exception("Token already expired! Please connect to VPN and login again");
                    }
                    else
                    {
                        var ssoToken = new SSOToken();
                        ssoToken.RefreshToken = userSSO.getDecryptedToken();
                        return ssoToken;
                    }
                }
                else
                {
                    throw new Exception($"No token found for {username}! Please connect to VPN and login again");
                }
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<string> GetRegisteredDeviceId()
        {
            try
            {
                using var dbContext = new DBContext();

                var tableName = ReflectionFactory.GetTableAttribute(typeof(ModTableMachineLog));
                var query = "select * from " + tableName;
                var result = (await dbContext.ReadMapperAsync<ModTableMachineLog>(query)).FirstOrDefault();
                var deviceId = Factory.Crypto.Cipher.Instance.DecryptString(result.Fingerprint);
                return deviceId;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
