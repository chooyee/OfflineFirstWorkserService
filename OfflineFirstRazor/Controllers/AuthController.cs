using Factory;
using Factory.Keycloak;
using Microsoft.AspNetCore.Mvc;
using Factory.Keycloak.Model;
using Model;
using Newtonsoft.Json;

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

        [HttpGet, Route("/sso/healthcheck")]
        public async Task<ActionResult<string>> SSOHealthCheck()
        {
            var result = await RHSSOLib.HealthCheck();

            return JsonConvert.SerializeObject(result);

        }

        [HttpPost, Route("/sso/client/login")]
		public async Task<ActionResult<SSOToken>> SSOUserLogin(string username, string userSecret)
		{
			//string username = "test001";
			//string userSecret = "1234";
            var token = await RHSSOLib.GetUserToken(Global.GlobalConfig.Instance.UserClient.Client_id, username, userSecret.ToSecureString());

			return token;

		}

		[HttpPost, Route("/sso/token/refresh")]
		public async Task<ActionResult<SSOToken>> SSOGetAccessToken(string refreshToken)
		{
			var token = await RHSSOLib.GetUserAccessToken(Global.GlobalConfig.Instance.UserClient.Client_id, refreshToken);

			return token;

		}

		[HttpPost, Route("/sso/token/azp")]
		public ActionResult<string> GetAzp(string refreshToken)
		{
			
			var token = RHSSOLib.DecodeJwt(refreshToken);

			return token;

		}

	}
}
