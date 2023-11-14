using System.Text.Json.Serialization;

namespace Factory.Keycloak.Model
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Account
    {
        [JsonPropertyName("roles")]
        public List<string> roles { get; set; }
    }

    public class RealmAccess
    {
        [JsonPropertyName("roles")]
        public List<string> roles { get; set; }
    }

    public class ResourceAccess
    {
        [JsonPropertyName("account")]
        public Account account { get; set; }
    }

    public class JwtToken
    {
        [JsonPropertyName("exp")]
        public int exp { get; set; }

        [JsonPropertyName("iat")]
        public int iat { get; set; }

        [JsonPropertyName("jti")]
        public string jti { get; set; }

        [JsonPropertyName("iss")]
        public string iss { get; set; }

        [JsonPropertyName("aud")]
        public string aud { get; set; }

        [JsonPropertyName("sub")]
        public string sub { get; set; }

        [JsonPropertyName("typ")]
        public string typ { get; set; }

        [JsonPropertyName("azp")]
        public string azp { get; set; }

        [JsonPropertyName("session_state")]
        public string session_state { get; set; }

        [JsonPropertyName("preferred_username")]
        public string preferred_username { get; set; }

        [JsonPropertyName("email_verified")]
        public bool email_verified { get; set; }

        [JsonPropertyName("acr")]
        public string acr { get; set; }

        [JsonPropertyName("allowed-origins")]
        public List<string> allowedorigins { get; set; }

        [JsonPropertyName("realm_access")]
        public RealmAccess realm_access { get; set; }

        [JsonPropertyName("resource_access")]
        public ResourceAccess resource_access { get; set; }

        [JsonPropertyName("scope")]
        public string scope { get; set; }

        [JsonPropertyName("sid")]
        public string sid { get; set; }

        [JsonPropertyName("clientHost")]
        public string clientHost { get; set; }

        [JsonPropertyName("clientAddress")]
        public string clientAddress { get; set; }

        [JsonPropertyName("client_id")]
        public string client_id { get; set; }

        [JsonPropertyName("username")]
        public string username { get; set; }

        [JsonPropertyName("active")]
        public bool active { get; set; }
    }


}
