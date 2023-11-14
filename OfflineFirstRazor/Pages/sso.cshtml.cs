using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Service;

namespace OfflineFirstRazor.Pages
{
    public class SSOParamModel
    {
        [BindProperty]
        public string AccessToken { get; set; }
        public string Sid{ get; set; }
        public string GotoPage { get; set; }
        public string UserName { get; set; }

    }

    public class ssoModel : PageModel
    {
        public void OnGet()
        {
            string accessToken = Request.Query["accessToken"].ToString();
            string sid = Request.Query["sid"].ToString();
            string gotoPage = Request.Query["gotopage"].ToString();
            try
            {
                var loginService = new OlifAuthService();
                var authResult = loginService.SSOLogin(sid, accessToken).Result;
              
                if (authResult.Item1)
                {
                    var loginSessionStr = JsonConvert.SerializeObject(authResult.Item3);
                    HttpContext.Session.SetString("UserName", authResult.Item3.UserName);
                    HttpContext.Session.SetString("loginSession", loginSessionStr);
                    Response.Redirect("home?gotoPage=" + gotoPage);
                }               
                else {
                    //not active token, redirect to 
                    throw new Exception(authResult.Item2);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("/?err=" + ex.Message);
            }
            
        }

        public async void OnPost(SSOParamModel ssoParam)
        {
            string accessToken = ssoParam.AccessToken;
            string sid = ssoParam.Sid;
            string gotoPage = ssoParam.GotoPage;
            try
            {
                var loginService = new OlifAuthService();
                var authResult = await loginService.SSOLogin(sid, accessToken);
                var jwtToken = await new RHSSOService().Introspect(accessToken);
                if (jwtToken.active)
                {

                    Response.Redirect("/home?goto=" + gotoPage);
                }
                else
                {
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
