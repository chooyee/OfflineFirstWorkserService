using System.Security.Cryptography;
using System.Text;

namespace Factory.Crypto
{
    public class Cipher
    {
        private const string container = "OlifContainer";
        public static string EncryptString(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            string publicKey = Keyman.GetPublicKey(container);

            using var rsa = new RSACryptoServiceProvider();
            
            rsa.FromXmlString(publicKey);

            byte[] encryptedData = rsa.Encrypt(dataBytes, true);

            return Convert.ToBase64String(encryptedData);
        }

        public static string DecryptString(string encryptedMsg)
        {
            var privateKeyXml = Keyman.GetPrivateKey(container);

            byte[] encryptedData = Convert.FromBase64String(encryptedMsg);

            using var rsa = new RSACryptoServiceProvider();
            
            // Import the private key
            rsa.FromXmlString(privateKeyXml);

            byte[] decryptedData = rsa.Decrypt(encryptedData, true);

            return Encoding.UTF8.GetString(decryptedData);

        }
    
    }
}
