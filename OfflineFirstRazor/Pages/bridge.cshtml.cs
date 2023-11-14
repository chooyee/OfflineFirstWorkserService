
using Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using Newtonsoft.Json;
using Serilog;
using Service;
using System.Diagnostics;



namespace OfflineFirstRazor.Pages
{
    public class BridgeModel
    {
        [BindProperty]
        public string Url { get; set; }

    }
    /// <summary>
    /// This is strictly for POC with WMS. This is not for Production!
    /// </summary>
    public class bridgeModel : PageModel
    {
        public void OnGet()
        {
            if (!HttpContext.Session.Keys.Contains("UserName"))
            {
                Response.Redirect("/?err=No session found");
            }
            //Message = $"Hello {HttpContext.Session.GetString("Username")}";
        }

        public async Task OnPost(BridgeModel bridgeParams)
        {
            try
            {
                var loginSessionStr = HttpContext.Session.GetString("loginSession");
                var loginSession = JsonConvert.DeserializeObject<LoginResultModel>(loginSessionStr);

                RHSSOService service = new RHSSOService();
                var ssoToken = await service.GetServiceToken();
                Response.Redirect($"{bridgeParams.Url}?sid={loginSession.Sid}&username={loginSession.UserName}&accesstoken={ssoToken.AccessTokenSecureString().ToCString()}");
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
