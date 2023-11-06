using Newtonsoft.Json;
using Factory.RHSSOService.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security;
using Serilog;
using System.Diagnostics;

namespace Factory.RHSSOService
{
    public static class RHSSOLib
    {
        public static async Task<SSOToken> GetUserToken(string client_id, string username, SecureString secret)
        {
			var ssoEndpoint = GlobalEnv.Instance.SSOEndpoint;


            string requestUri = ssoEndpoint.Http + "://" + ssoEndpoint.AbsUrl+ ssoEndpoint.Auth;
            var param = new Dictionary<string, string>
            {
                { "client_id", client_id },
                { "username", username },
                { "password", secret.ToCString() },
                { "grant_type", "password" }
            };

            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };
            
            using (HttpClient client = new HttpClient(clientHandler))
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(requestUri, new FormUrlEncodedContent(param));

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<SSOToken>(await response.Content.ReadAsStringAsync());

                    }
                    else
                    {
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
                catch(Exception ex) {
                    var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    Log.Error("{funcName}: {error}", funcName, ex.Message);
                    throw new Exception(ex.Message);
                }
               
            }
        }

		public static async Task<SSOToken> GetUserAccessToken(string client_id, string refreshToken)
		{
			var ssoEndpoint = GlobalEnv.Instance.SSOEndpoint;

			string requestUri = ssoEndpoint.Http + "://" + ssoEndpoint.AbsUrl + ssoEndpoint.Auth;
			var param = new Dictionary<string, string>
			{
				{ "client_id", client_id },
				{ "grant_type", "refresh_token" },
				{ "refresh_token", refreshToken }
			};

			HttpClientHandler clientHandler = new HttpClientHandler()
			{
				ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
			};

			using (HttpClient client = new HttpClient(clientHandler))
			{
                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(requestUri, new FormUrlEncodedContent(param));

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<SSOToken>(await response.Content.ReadAsStringAsync());

                    }
                    else
                    {
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
                catch (Exception ex)
                {
                    var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    Log.Error("{funcName}: {error}", funcName, ex.Message);
                    throw new Exception(ex.Message);
                }

			}


		}

		public static async Task<SSOToken> GetServiceToken(string client_id, string client_secret)
        {
			var ssoEndpoint = GlobalEnv.Instance.SSOEndpoint;

			string url = ssoEndpoint.Http + "://" + ssoEndpoint.AbsUrl + ssoEndpoint.Auth;
            

            SSOToken token = new SSOToken();
              

            string ssoCredentialStr = client_id + ":" + client_secret;
            string authToken64 = "Basic " + Base64Encode(ssoCredentialStr);


            //string formBody = "grant_type=client_credentials";
			var formContent = new FormUrlEncodedContent(new[]
			{
					new KeyValuePair<string, string>("grant_type", "client_credentials")
			});

			var handler = new HttpClientHandler()
			{
				ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			};


			using (HttpClient client = new HttpClient(handler))
			{
                try
                {
                    client.DefaultRequestHeaders.Add("Authorization", authToken64);
                    HttpResponseMessage response = await client.PostAsync(url, formContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<SSOToken>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
                catch (Exception ex)
                {
                    var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    Log.Error("{funcName}: {error}", funcName, ex.Message);
                    throw new Exception(ex.Message);
                }

			}

        }

        public static async Task<JwtToken> Introspect(string client_id, string client_secret, string accessToken)
        {
            var ssoEndpoint = GlobalEnv.Instance.SSOEndpoint;
            string ssoCredentialStr = client_id + ":" + client_secret;
            string authToken64 = "Basic " + Base64Encode(ssoCredentialStr);

            string url = ssoEndpoint.Http + "://" + ssoEndpoint.AbsUrl + ssoEndpoint.Introspect;

            var formContent = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("token", accessToken)
            });


            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };


            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    client.DefaultRequestHeaders.Add("Authorization", authToken64);
                    HttpResponseMessage response = await client.PostAsync(url, formContent);

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<JwtToken>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
                catch (Exception ex)
                {
                    var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    Log.Error("{funcName}: {error}", funcName, ex.Message);
                    throw new Exception(ex.Message);
                }

            }
        }

        public static string DecodeJwt(string stream)
        {
			
			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(stream);
			var tokenS = jsonToken as JwtSecurityToken;
			var  azp = tokenS.Claims.First(claim => claim.Type == "azp").Value;
           
			return azp;
		}
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        
    }

   
}
