using Factory;
using Factory.RHSSOService.Model;
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
    public class LoginResultModel
    {
        private string _username;
        private string _domain;
        private readonly string _deviceId;
        private LoginStatus _loginStatus;
        private bool _ssoHealthStatus;
        private SSOToken _token;
        private DateTime _loginDate;

        public string UserName { get { return _username; } }
       
        public string Domain { get { return _domain; } }

        public string DeviceId{ get { return _deviceId; } }

        public LoginStatus LoginStatus { get { return _loginStatus; } set { _loginStatus = value; } }
        public bool SSOHealthStatus { get { return _ssoHealthStatus; } set { _ssoHealthStatus = value; } }
        public SSOToken Token { get { return _token; } set { _token = value; } }
        public DateTime LoginDate{ get { return _loginDate; } }


        public LoginResultModel(string username, string domain)
        { 
            _username = username;
            _domain = domain;           
            _deviceId = Fingerprint.GenFingerprint();
            _loginDate = DateTime.Now;
        }
    }

    
}
