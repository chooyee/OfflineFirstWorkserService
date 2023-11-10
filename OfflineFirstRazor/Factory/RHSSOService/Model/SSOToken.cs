using Newtonsoft.Json;
using System.Security;

namespace Factory.RHSSOService.Model
{
    public class SSOToken
    {
        private SecureString? _accessToken;
        private SecureString? _refreshToken;

        [JsonProperty("access_token")]
        public string? AccessToken {
            set {
                if (value == null) throw new ArgumentNullException(nameof(AccessToken));
                _accessToken = value.ToSecureString();
            }
        }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("refresh_expires_in")]
        public long RefreshExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string? RefreshToken
        {
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(RefreshToken));
                _refreshToken = value.ToSecureString();
            }
        }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("not-before-policy")]
        public long NotBeforePolicy { get; set; }

        [JsonProperty("session_state")]
        public Guid SessionState { get; set; }

        public SecureString AccessTokenSecureString()
        {
            return _accessToken;
        }

        public SecureString RefreshTokenSecureString()
        {
            return _refreshToken;
        }

    }


}
