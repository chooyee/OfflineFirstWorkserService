using Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Model;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public ActionResult<bool> Login(LoginModel loginRequest)
        {
            var winAuth = new WinAuth();
            return winAuth.Auth(loginRequest.UserName, loginRequest.Domain, loginRequest.GetPasswordAsSecureString());
        }
    }
}
