using System.Text;

namespace Model.SSO
{
    public class SSOEndpoint:ISSOConfig
    {
        public string Http { get; set; }
        public string AbsUrl { get; set; }
        public string Auth { get; set; }
        public string Introspect { get; set; }
        public string HealthCheck { get; set; }
        public string Realm { get; set; }

        public SSOEndpoint() { }


        public static string GetRealm(string realmName)
        {
            return string.Format("/auth/admin/realms/{0}", realmName);

        }
    }

    internal static class SSOEndpointExtension
    {
        internal static SSOEndpoint AsSSOEndpoint(this ISSOConfig ssoConfig)
        {
            SSOEndpoint ssoEndpoint = new SSOEndpoint();
            ssoEndpoint.Http = ssoConfig.Http;
            ssoEndpoint.AbsUrl = GetAbsUrl(ssoConfig.AbsUrl);
            ssoEndpoint.Auth = GetRealm(ssoConfig.Auth, ssoConfig.Realm);
            ssoEndpoint.Introspect = GetRealm(ssoConfig.Introspect, ssoConfig.Realm);
            ssoEndpoint.HealthCheck = GetRealm(ssoConfig.HealthCheck, ssoConfig.Realm);
            ssoEndpoint.Realm= ssoConfig.Realm;
            return ssoEndpoint;
        }

        private static string GetRealm(string endpoint, string realm)
        {
            StringBuilder stringBuilder = new StringBuilder(endpoint);
            stringBuilder.Replace("{realm}", realm);

            // Get the final result as a string
            return stringBuilder.ToString();
        }

        private static string GetAbsUrl(string absUrl)
        {
            return Cryptolib2.Crypto.DecryptText(absUrl, false);
        }
    }
}
