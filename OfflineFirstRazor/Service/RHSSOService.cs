using Factory;
using Factory.Keycloak;
using Factory.Keycloak.Model;
using Global;
using System.Security;

namespace Service
{
    internal sealed class RHSSOService
    {
        
        internal RHSSOService()
        {
       
        }

        internal async Task<SSOToken> GetServiceToken()
        {
            string clientId = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.ServiceClient.Client_id ?? "", false);
            SecureString clientSecret = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.ServiceClient.Client_secret?? "");
            return await RHSSOLib.GetServiceToken(clientId, clientSecret);
        }

        internal async Task<JwtToken> Introspect(string accessToken)
        {
            string clientId = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.ServiceClient.Client_id ?? "", false);
            SecureString clientSecret = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.ServiceClient.Client_secret ?? "");
            return await RHSSOLib.Introspect(clientId, clientSecret, accessToken);
        }
    }
}
