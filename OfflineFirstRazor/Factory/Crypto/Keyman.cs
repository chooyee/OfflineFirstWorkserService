using Serilog;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Cryptography;

namespace Factory.Crypto
{
    [SupportedOSPlatform("windows")]
    internal class Keyman
    {
        internal static string GetPublicKey(string containerName)
        {
            try
            {
                // Create the CspParameters object and set the key container
                // name used to store the RSA key pair.
                var parameters = new CspParameters
                {
                    KeyContainerName = containerName
                };

                using var rsa = new RSACryptoServiceProvider(parameters);
                return rsa.ToXmlString(false);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        internal static string GetPrivateKey(string containerName)
        {
            try
            {
                // Create the CspParameters object and set the key container
                // name used to store the RSA key pair.
                var parameters = new CspParameters
                {
                    KeyContainerName = containerName
                };

                using var rsa = new RSACryptoServiceProvider(parameters);
                return rsa.ToXmlString(true);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }



        internal static bool DeleteKey(string containerName)
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
                using var rsa = new RSACryptoServiceProvider(parameters)
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
    }
}
