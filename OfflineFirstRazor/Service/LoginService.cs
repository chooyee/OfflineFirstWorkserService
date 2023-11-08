using Factory;
using Factory.DB;
using Factory.RHSSOService;
using Factory.RHSSOService.Model;
using Model;
using Serilog;
using System.Diagnostics;
using System.Security;

namespace Service
{
    public class LoginService
    {

        public LoginService() { }

        public async Task<LoginResultModel> Login(string username, SecureString password, string domain = "")
        {          
            try
            {
                var ssoHealth = await RHSSOLib.HealthCheck();
                if (ssoHealth.Item1)
                {
                    return await SSOLogin(username, password, domain);
                }
                else
                {
                    return WindowsLogin(username, password, domain);
                }
                
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task<LoginResultModel> SSOLogin(string username, SecureString password, string domain)
        {
            try
            {
                var ssoToken = await RHSSOLib.GetUserToken(GlobalEnv.Instance.UserClient.Client_id, username, password);
                await SSOTokenLog(ssoToken, username);
                return GenLoginResult(username, LoginType.SSO, domain, ssoToken.AccessToken, true);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                return GenLoginResult(username, LoginType.SSO, domain, ex.Message, false);
            }
        }

        private LoginResultModel WindowsLogin(string username, SecureString password, string domain)
        {
            var authResult = new WinAuth().Auth(username, domain, password);
            return GenLoginResult(username, LoginType.SSO, domain, "", authResult);
        }

        private async Task<int> SSOTokenLog(SSOToken ssoToken, string username)
        {
            try
            {
                using (var dbContext = new DBContext())
                {
                    var tokenExpiryDate = DateTime.Now.AddSeconds(ssoToken.RefreshExpiresIn);
                    var userSSO = new ModTableSSOUser(username, ssoToken.AccessToken, tokenExpiryDate);

                    var query = dbContext.QueryFactory.Insert(userSSO);
                    return await dbContext.ExecuteNonQueryAsync(query.Item1, query.Item2);
                }
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private LoginResultModel GenLoginResult(string username, LoginType loginType, string domain,string accessToken, bool loginStatus)
        {
            return new LoginResultModel(username, loginType, domain, accessToken, loginStatus);
           
        }
    }
}
