using Factory;
using Factory.Keycloak;
using Microsoft.AspNetCore.Mvc;
using Factory.Keycloak.Model;
using Model;
using Newtonsoft.Json;
using Service;

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
			var ssoService = new RHSSOService();
            var result = await ssoService.HealthCheck();

            return JsonConvert.SerializeObject(result);

        }

        [HttpPost, Route("/sso/client/login")]
		public async Task<ActionResult<SSOToken>> SSOUserLogin(string username, string userSecret)
		{
            //string username = "test001";
            //string userSecret = "1234";
            var ssoService = new RHSSOService();
            var token = await ssoService.GetUserToken(username, userSecret.ToSecureString());

			return token;

		}

		[HttpPost, Route("/sso/token/refresh")]
		public async Task<ActionResult<SSOToken>> SSOGetAccessToken(string refreshToken)
		{
            var ssoService = new RHSSOService();
            var token = await ssoService.GetUserToken(refreshToken);

			return token;

		}

		[HttpPost, Route("/sso/token/azp")]
		public ActionResult<string> GetAzp(string refreshToken)
		{
           
            var token = KeycloakApiFactory.DecodeJwt(refreshToken);

			return token;

		}

	}
}
