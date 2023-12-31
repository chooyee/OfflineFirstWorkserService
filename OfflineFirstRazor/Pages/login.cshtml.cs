using Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.DirectoryServices.ActiveDirectory;
using WebApi.Model;

namespace OfflineFirstRazor.Pages
{
    public class loginModel : PageModel
    {
        public string Message { get; set; }
        public string Domain { get; set; }

        public loginModel()
        {
            Domain = Factory.DomainHelper.GetPCDomainName();
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

        public void OnPost()
        {
            try
            {
                var loginRequest = new LoginModel()
                {
                    Domain = Request.Form["domain"],
                    UserName = Request.Form["username"],
                    Password = Request.Form["password"]
                };

                var authResult = new WinAuth().Auth(loginRequest.UserName, loginRequest.Domain, loginRequest.GetPasswordAsSecureString());
                if (authResult)
                {
                    HttpContext.Session.SetString("UserName", loginRequest.UserName);
                    Response.Redirect("home");
                }
                else
                {
                    Message = "Login failed!";
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

    }
}
