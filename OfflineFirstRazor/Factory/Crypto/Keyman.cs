using Serilog;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace Factory.Crypto
{
    [SupportedOSPlatform("windows")]
    public class Keyman
    {
        private const string container = "OlifContainer";
        private const int keySize = 2048;
        public void InitKey()
        {
            //DeleteKey(container);
            GetPrivateKey(container);
            if (GetSalt() == "") { RegisterSalt(); }
        }

        public string GetPublicKey()
        {
            return GetPublicKey(container);
        }

        public string GetPrivateKey()
        {
            return GetPrivateKey(container);
        }

        private string GetPublicKey(string containerName)
        {
            try
            {
                // Create the CspParameters object and set the key container
                // name used to store the RSA key pair.
                var parameters = new CspParameters
                {
                    KeyContainerName = containerName
                };

                using var rsa = new RSACryptoServiceProvider(keySize, parameters);
                return rsa.ToXmlString(false);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        protected string GetPrivateKey(string containerName)
        {
            try
            {
                // Create the CspParameters object and set the key container
                // name used to store the RSA key pair.
                var parameters = new CspParameters
                {
                    KeyContainerName = containerName                    
                };

                using var rsa = new RSACryptoServiceProvider(keySize, parameters);
                return rsa.ToXmlString(true);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        protected bool DeleteKey(string containerName)
        {
            try
            {
                // Create the CspParameters object and set the key container
                // name used to store the RSA key pair.
                var parameters = new CspParameters
                {
                    KeyContainerName = containerName
                };

                // Create a new instance of RSACryptoServiceProvider that accesses
                // the key container.
                using var rsa = new RSACryptoServiceProvider(keySize, parameters)
                {
                    // Delete the key entry in the container.
                    PersistKeyInCsp = false
                };

                // Call Clear to release resources and delete the key from the container.
                rsa.Clear();

                Console.WriteLine("Key deleted.");
                return true;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                return false;
            }
        }

        private bool RegisterSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[1024];

            rng.GetBytes(buffer);
            string salt = BitConverter.ToString(buffer);

            var registry = new WinRegistryFactory();
            return registry.SetValue("sid", Sha256(salt));
        }

        protected string GetSalt()
        {
            var registry = new WinRegistryFactory();
            return registry.GetValue("sid");
        }

        private string Sha256(string input) {
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
