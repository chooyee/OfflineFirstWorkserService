using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Factory
{
    public sealed class GlobalEnv
    {
        public static GlobalEnv Instance { get; private set; }
		private readonly string env;
		private readonly SSOEndpointStruct ssoEndpoint;
		private readonly SSOCredential userClient;
		private readonly SSOCredential serviceClient;
        private GlobalEnv()
        {
            var env = ConfigurationManager.AppSettings["env"];

			ssoEndpoint = new SSOEndpointStruct();
			ssoEndpoint.Http = ConfigurationManager.AppSettings.Get($"{env}.sso.http") ?? "";
			ssoEndpoint.AbsUrl = ConfigurationManager.AppSettings.Get($"{env}.sso.absurl")??"";
			ssoEndpoint.Auth = ConfigurationManager.AppSettings.Get($"{env}.sso.auth") ?? "";
			ssoEndpoint.Introspect = ConfigurationManager.AppSettings.Get($"{env}.sso.introspect") ?? "";
			ssoEndpoint.Realm = ConfigurationManager.AppSettings.Get($"{env}.sso.realm") ?? "";

			ssoEndpoint.Auth = ssoEndpoint.Auth.Replace("{realm}", ssoEndpoint.Realm);
			ssoEndpoint.Introspect = ssoEndpoint.Introspect.Replace("{realm}", ssoEndpoint.Realm);

			userClient.Client_id = ConfigurationManager.AppSettings.Get($"{env}.sso.user.client_id") ?? "";
			serviceClient.Client_id = ConfigurationManager.AppSettings.Get($"{env}.sso.service.client_id") ?? "";
			serviceClient.Client_secret= ConfigurationManager.AppSettings.Get($"{env}.sso.service.client_secret") ?? "";
		}

        public SSOEndpointStruct SSOEndpoint { get { return ssoEndpoint; } }
        public string Environment { get { return env; } }
		public SSOCredential UserClient { get { return userClient; } }
		public SSOCredential ServiceClient { get { return serviceClient; } }

		static GlobalEnv() { Instance = new GlobalEnv(); }
    }

    public struct EndpointStruct
    {
        public string Http { get; set; }
        public string AbsUrl { get; set; }
        public string Endpoint { get; set; }

        public string GetEndpoint()
        {
            return Http + "://" + AbsUrl + Endpoint;
        }
    }

	public struct SSOEndpointStruct
	{
		public string Http { get; set; }
		public string AbsUrl { get; set; }
		public string Auth { get; set; }
		public string Introspect { get; set; }
		public string Realm { get; set; }

		public static string GetRealm(string realmName)
		{
			return string.Format("/auth/admin/realms/{0}", realmName);

		}
	}

	public struct SSOCredential
	{
		public string Client_id { get; set; }
		public string Client_secret { get; set; }
	}
}
