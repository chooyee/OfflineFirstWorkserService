using Factory;
using System.Security;

namespace Model
{
    public enum LoginType
    {
        Windows,
        SSO,
        None
    };

    public struct LoginModel
    {
        private SecureString _password;

        public string UserName { get; set;}
        public string Password
        {
            get {
                return "";
            }
            set
            {
                _password = value.ToSecureString();
            }
        }
        public string Domain{ get; set;}

        public string GetPasswordAsString()
        {
            return _password.ToCString();
        }

        public SecureString GetPasswordAsSecureString()
        {
            return _password;
        }

        public void SetPassword(SecureString securePwd)
        {
            _password = securePwd;
        }
    }

    public struct LoginResultModel
    {
        private string _username;
        private LoginType _loginType;
        private string _domain;
        private string _accessToken;
        private bool _active = false;

        public readonly string UserName { get { return _username; } }
        
        public readonly LoginType LoginType { get { return _loginType; } }

        public readonly string Domain { get { return _domain; } }

        public readonly string AccessToken{ get { return _accessToken; } }

        public readonly bool Active{ get { return _active; } }

        public LoginResultModel(string username, LoginType loginType, string domain, string accessToken, bool active)
        { 
            _username = username;
            _loginType = loginType;
            _domain = domain;
            _accessToken = accessToken;
            _active = active;
        }
    }
}
