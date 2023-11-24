using Factory.Keycloak;
using Factory.Keycloak.Model;
using Global;
using Model.SSO;
using System.Security;

namespace Service
{
    internal sealed class RHSSOService
    {
        private SSOEndpoint ssoEndpoint;

        internal RHSSOService()
        {
            ssoEndpoint = GlobalConfig.Instance.SSOConfig.AsSSOEndpoint();
        }

        /// <summary>
        /// HealthCheck - Check is current RHSSO/Keycloak is up
        /// </summary>
        /// <returns>Tuple<bool (status), string (Error Message, if any)></returns>
        internal async Task<Tuple<bool, string>> HealthCheck()
        {
            return await KeycloakApiFactory.HealthCheck(ssoEndpoint);
        }

        /// <summary>
        /// GetUserToken - Get Access Token on behalf of user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userSecret"></param>
        /// <returns>SSOToken</returns>
        internal async Task<SSOToken> GetUserToken(string username, SecureString userSecret)
        {
            string clientId = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.UserClient.Client_id ?? "", false);
            //SecureString clientSecret = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.ServiceClient.Client_secret ?? "");
            return await KeycloakApiFactory.GetUserToken(ssoEndpoint, clientId, username, userSecret);
        }

        /// <summary>
        /// GetUserToken - Get Access Token on behalf of user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userSecret"></param>
        /// <returns>SSOToken</returns>
        internal async Task<SSOToken> GetUserToken(string refreshToken)
        {
            string clientId = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.UserClient.Client_id ?? "", false);
            //SecureString clientSecret = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.ServiceClient.Client_secret ?? "");
            return await KeycloakApiFactory.GetUserAccessToken(ssoEndpoint, clientId, refreshToken);
        }

        /// <summary>
        /// GetServiceToken
        /// </summary>
        /// <returns>SSOToken</returns>
        internal async Task<SSOToken> GetServiceToken()
        {
            string clientId = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.ServiceClient.Client_id ?? "", false);
            SecureString clientSecret = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.ServiceClient.Client_secret?? "");
            return await KeycloakApiFactory.GetServiceToken(ssoEndpoint, clientId, clientSecret);
        }

        /// <summary>
        /// Introspect
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns>JwtToken</returns>
        internal async Task<JwtToken> Introspect(string accessToken)
        {
            string clientId = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.ServiceClient.Client_id ?? "", false);
            SecureString clientSecret = Cryptolib2.Crypto.DecryptText(GlobalConfig.Instance.ServiceClient.Client_secret ?? "");
            return await KeycloakApiFactory.Introspect(ssoEndpoint, clientId, clientSecret, accessToken);
        }
    }
}
