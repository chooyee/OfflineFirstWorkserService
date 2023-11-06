using Factory;
using Factory.RHSSOService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OfflineFirstRazor.Pages
{
    public class ssoModel : PageModel
    {
        public void OnGet()
        {
            string accessToken = Request.Query["accessToken"];
            try
            {
                var jwtToken = RHSSOLib.Introspect(GlobalEnv.Instance.ServiceClient.Client_id, GlobalEnv.Instance.ServiceClient.Client_secret, accessToken).Result;
                if (jwtToken.active)
                {
                    //
                    // Get refresh token from DB --jwtToken.username
                    // 

                    Response.Redirect("/home?accessToken=" + accessToken);
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
