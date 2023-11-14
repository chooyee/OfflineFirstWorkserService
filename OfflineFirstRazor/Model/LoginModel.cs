using Factory;
using Factory.DB.Model;
using Factory.RHSSOService.Model;
using System;
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

    public enum LoginStatus
    {
        SSOAuthActive,        
        OfflineAuthActive,
        AuthFailed
    }
    public class LoginResultModel: UserLoginLog
    {
        public long LoginUnixTimestamp { 
            get {
                DateTimeOffset dto = new DateTimeOffset(LoginDate.ToUniversalTime());
                return dto.ToUnixTimeSeconds();
            }
            set { }
        }

        public LoginResultModel()
        {
        }

        public LoginResultModel(string sid)
        {
            Sid = sid;
        }

        public LoginResultModel(string username, string domain)
        {
            Sid = Guid.NewGuid().ToString();
            UserName = username;
            Domain = domain;
            LoginDate = DateTime.Now;
        }
    }

    
}
