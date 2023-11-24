using Factory;
using Factory.Crypto;
using Model;
using Serilog;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Service
{
    /// <summary>
    /// CipherService - In order to create cipher service, system need to authenticate user locally
    /// Set to Internal to prevent library accessed by third party software
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal class CipherService:Cipher
    {
        /// <summary>
        /// GetSessionId - Create Session ID
        /// </summary>
        /// <param name="loginSession"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns>Tuple<bool, string></returns>
        internal static Tuple<bool, string> GetSessionId(LoginResultModel loginSession, string username, SecureString password, string domain)
        {
            try
            {
                if (OlifAuthService.WindowsAuthentication(username, password, domain))
                {
                    return Tuple.Create(true, new CipherService().EncryptString($"{loginSession.LoginUnixTimestamp}{password.ToCString()}"));
                }
                else
                {
                    return Tuple.Create(false, "Invalid username or password!");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// GetCipherService - Return cipher service for valid login session. 
        /// You can get login session from the return of OlifAuthService Login function. 
        /// </summary>
        /// <param name="loginSession">LoginResultModel</param>
        /// <returns>CipherService</returns>
        /// <exception cref="Exception"></exception>
        internal static CipherService GetCipherService(LoginResultModel loginSession)
        {
            
            try
            {
                var descryptedSession = new CipherService().DecryptString(loginSession.SessionId);
                var sessionUnixTimestamp = GetLoginTimestamp(loginSession.LoginUnixTimestamp, descryptedSession);
                if (sessionUnixTimestamp != loginSession.LoginUnixTimestamp)
                {
                    var errMsg = $"Session Timestamp [{sessionUnixTimestamp}] and Login timestamp [{loginSession.LoginUnixTimestamp}] does not match!";
                  
                    throw new Exception(errMsg);
                }

                //Authenticate
                if (OlifAuthService.WindowsAuthentication(loginSession.UserName, GetPassword(loginSession.LoginUnixTimestamp, descryptedSession), loginSession.Domain))
                {
                    return new CipherService();
                }
                else
                {
                    var errMsg = $"{loginSession.UserName}: Windows authentication failed!";                   
                    throw new Exception(errMsg);
                }
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception("Invalid session", new Exception(ex.Message));
            }
        }

        private static long GetLoginTimestamp(long unixTimestamp,string session)
        {
            return long.Parse(session.Substring(0, unixTimestamp.ToString().Length));
        }

        private static SecureString GetPassword(long unixTimestamp, string session)
        {
            return session.Substring(unixTimestamp.ToString().Length).ToSecureString();
        }

        /// <summary>
        /// Encrypt - return as base64 string
        /// Max encryption data length is 200
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Encrypted Base64 string</returns>
        internal string Encrypt(string data)
        {
            return base.EncryptString(data);
        }

        /// <summary>
        /// Decrypt 
        /// </summary>
        /// <param name="encryptedBase64Str">Encrypted string in base 64</param>
        /// <returns>string</returns>
        internal string Decrypt(string encryptedBase64Str)
        {
            return base.DecryptString(encryptedBase64Str);
        }

        /// <summary>
        /// EncryptLongString - RSA encryption for Key and IV.
        /// AES encryption for long string
        /// </summary>
        /// <param name="data"></param>
        /// <returns>EncryptResultModel - Encrypted Key, IV and data</returns>
        /// <exception cref="CryptographicException"></exception>
        internal EncryptResultModel EncryptLongString(string data)
        {
            try
            {
                using Aes aes = Aes.Create();
                aes.KeySize = 256;
                aes.GenerateKey();
                aes.GenerateIV();

                var encryptedBase64 = Convert.ToBase64String(AesEncryptString(data, aes.Key, aes.IV));

                var result = new EncryptResultModel()
                {
                    EncryptedKeyBase64Str = EncryptString(Encoding.UTF8.GetString(aes.Key)),
                    EncryptedIVBase64Str = EncryptString(Encoding.UTF8.GetString(aes.IV)),
                    EncryptedBase64Str = encryptedBase64
                };

                return result;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new CryptographicException($"{funcName}: {ex.Message}");
            }
        }

        internal string DecryptLongString(EncryptResultModel encryptionResult)
        {
            try
            {
                var encryptedData = Convert.FromBase64String(encryptionResult.EncryptedBase64Str);

                var key = Encoding.UTF8.GetBytes(DecryptString(encryptionResult.EncryptedKeyBase64Str));
                var iv = Encoding.UTF8.GetBytes(DecryptString(encryptionResult.EncryptedIVBase64Str));

                return AesDecryptString(encryptedData, key, iv);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new CryptographicException($"{funcName}: {ex.Message}");
            }
        }

        private CipherService()
        {
            salt = GetSalt();
        }


        /// <summary>
        /// InitKey - Only call during first startup
        /// </summary>
        internal static void InitKey()
        {
            //DeleteKey(container);
            var cs = new CipherService();
            cs.GetPrivateKey(container);
            if (GetSalt() == "") { RegisterSalt(); }
        }

        internal static void UninstallKey()
        {           
            var cs = new CipherService();
            cs.DeleteKey(container);
            DeleteSalt();
        }

        private static bool RegisterSalt()
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[1024]; // Adjust the size of the array based on your requirements

                // Fill the array with random bytes
                rng.GetBytes(randomBytes);

                // Convert the random bytes to an integer (or any other data type as needed)
                //int randomNumber = BitConverter.ToInt32(randomBytes, 0);
                string salt = BitConverter.ToString(randomBytes);
                var registry = new WinRegistryFactory();
                return registry.SetValue("sid", Sha256(salt));
            }

            //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            //byte[] buffer = new byte[1024];

            //rng.GetBytes(buffer);
            //string salt = BitConverter.ToString(buffer);

            //var registry = new WinRegistryFactory();
            //return registry.SetValue("sid", Sha256(salt));
        }

        protected static string GetSalt()
        {
            var registry = new WinRegistryFactory();
            return registry.GetValue("sid");
        }

        protected static bool DeleteSalt()
        {
            var registry = new WinRegistryFactory();
            return registry.DeleteKeyPath();
        }

        private static string Sha256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Convert the byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // "x2" formats the byte to a two-digit hexadecimal string
                }

                return builder.ToString();
            }
        }
    }
}
