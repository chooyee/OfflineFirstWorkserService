using Serilog;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace Factory.Crypto
{
    [SupportedOSPlatform("windows")]
    public class Cipher:Keyman
    {
        private readonly string salt;

        public static Cipher Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<Cipher> lazy = new Lazy<Cipher>();

        public Cipher()
        {
            salt = GetSalt();
        }

        public string EncryptString(string data)
        {
            try
            {
                // Combine data with the salt
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes($"{data}-{salt}");

                // Get the public key
                string publicKey = GetPublicKey();

                // Initialize the RSACryptoServiceProvider
                using (var rsa = new RSACryptoServiceProvider())
                {
                    // Load the public key
                    rsa.FromXmlString(publicKey);

                    byte[] encryptedData;

                    int maxLength = (rsa.KeySize / 8) - 11; // 11 is the PKCS#1 padding overhead

                    // Check if the data is within the maximum size for encryption
                    if (dataToEncrypt.Length <= maxLength)
                    {
                        encryptedData = rsa.Encrypt(dataToEncrypt, true);
                        return Convert.ToBase64String(encryptedData);
                    }
                    else
                    {
                        throw new Exception($"Data '{data}-{salt}' length {dataToEncrypt.Length} is too long for encryption using this key size. Maxlength: {maxLength}");
                    }
          
                }
            }
            catch (CryptographicException ex)
            {
                // Log the exception and re-throw a more general CryptographicException
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new CryptographicException("Error in encryption");
            }
            catch (Exception ex)
            {
                // Log other exceptions and throw a CryptographicException
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new CryptographicException("An unexpected error occurred");
            }
        }

        public string DecryptString(string encryptedData)
        {
            try
            {
                // Get the private key
                string privateKey = GetPrivateKey();

                // Initialize the RSACryptoServiceProvider
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(privateKey);

                    // Convert the encrypted data back to bytes
                    byte[] dataToDecrypt = Convert.FromBase64String(encryptedData);

                    // Decrypt the data
                    byte[] decryptedData = rsa.Decrypt(dataToDecrypt, true);

                    // Convert the decrypted bytes back to string
                    string decryptedText = Encoding.UTF8.GetString(decryptedData);

                    // Extract original data by splitting on the salt
                    string originalData = decryptedText.Split(new[] { '-' }, 2)[0];

                    return originalData;
                }
            }
            catch (CryptographicException ex)
            {
                // Log the exception and re-throw a more general CryptographicException
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new CryptographicException("Error in decryption");
            }
            catch (Exception ex)
            {
                // Log other exceptions and throw a CryptographicException
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new CryptographicException("An unexpected error occurred");
            }
        }

    }
}
