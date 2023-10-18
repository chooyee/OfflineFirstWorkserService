using Factory;
using System.Security;

namespace WebApi.Model
{
    public struct LoginModel
    {
        private string _username;
        private SecureString _password;
        private string _domain;

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
    }
}
