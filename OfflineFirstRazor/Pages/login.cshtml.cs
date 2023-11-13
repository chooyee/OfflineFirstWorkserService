using Factory;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using Service;

namespace OfflineFirstRazor.Pages
{
    public class loginModel : PageModel
    {
        public string Message { get; set; }
        public string Domain { get; set; }

        public loginModel()
        {
            Domain = DomainHelper.GetPCDomainName();
        }

        public void OnGet()
        {
            //Console.WriteLine("aaa");
            Message = " Hey Stranger, welcome to login page";
            if (Request.Query.ContainsKey("err"))
            {
                Message = Request.Query["err"].ToString();
            }
        }

        public async Task OnPost()
        {
            try
            {
                var loginRequest = new LoginModel()
                {
                    Domain = Request.Form["domain"],
                    UserName = Request.Form["username"],
                    Password = Request.Form["password"]
                };
                var loginService = new OlifAuthService();
                var authResult = await loginService.Login(loginRequest.UserName, loginRequest.GetPasswordAsSecureString(), loginRequest.Domain);
                //var authResult = new WinAuth().Auth(loginRequest.UserName, loginRequest.Domain, loginRequest.GetPasswordAsSecureString());
                if (authResult.Item1)
                {
                    HttpContext.Session.SetString("UserName", loginRequest.UserName);
                    Response.Redirect("home");
                }
                else
                {
                    Message = authResult.Item2;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

    }
}
