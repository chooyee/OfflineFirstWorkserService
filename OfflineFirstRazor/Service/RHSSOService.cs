using Factory;
using Factory.Keycloak;
using Factory.Keycloak.Model;
using ConfigurationManager = System.Configuration.ConfigurationManager;
namespace Service
{
    internal sealed class RHSSOService
    {
        private readonly SSOCredential serviceClient;

        internal RHSSOService()
        {
            var env = GlobalEnv.Instance.Environment;
            serviceClient.Client_id = Cryptolib2.Crypto.DecryptText(ConfigurationManager.AppSettings.Get($"{env}.sso.service.client_id") ?? "", false);
            serviceClient.Client_secret= Cryptolib2.Crypto.DecryptText(ConfigurationManager.AppSettings.Get($"{env}.sso.service.client_secret") ?? "");
        }

        internal async Task<SSOToken> GetServiceToken()
        {
            return await RHSSOLib.GetServiceToken(serviceClient.Client_id, serviceClient.Client_secret);
        }

        internal async Task<JwtToken> Introspect(string accessToken)
        {
            return await RHSSOLib.Introspect(serviceClient.Client_id, serviceClient.Client_secret, accessToken);
        }
    }
}
