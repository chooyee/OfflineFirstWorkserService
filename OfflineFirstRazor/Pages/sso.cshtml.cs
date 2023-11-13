using Factory;
using Factory.RHSSOService;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace OfflineFirstRazor.Pages
{
    public class ssoModel : PageModel
    {
        public async void OnGet()
        {
            string accessToken = Request.Query["accessToken"].ToString();
            string sessionId = Request.Query["sessionId"].ToString();
            string gotoPage = Request.Query["goto"].ToString();
            try
            {
                var loginService = new OlifAuthService();
                var authResult = await loginService.SSOLogin(int.Parse(sessionId), accessToken);
                var jwtToken = RHSSOLib.Introspect(GlobalEnv.Instance.ServiceClient.Client_id, GlobalEnv.Instance.ServiceClient.Client_secret, accessToken).Result;
                if (jwtToken.active)
                {
                   
                    Response.Redirect("/home?goto=" + gotoPage);
                }
                else {
                    //not active token, redirect to 
                    throw new Exception("Token session expired!");
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("/?err=" + ex.Message);
            }
            
        }
    }
}
