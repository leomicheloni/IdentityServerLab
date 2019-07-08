using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebClient
{
    public class HomeController: Controller
    {
        private string authserverurl = "http://localhost:5000";
        public IActionResult Index()
        {
            return RedirectToAction("BeginCodeFlowAuth");
        }


        public IActionResult BeginCodeFlowAuth()
        {
            var state = Guid.NewGuid().ToString("N");

            var url = $"{authserverurl}/connect/authorize"+
                        "?client_id=cf.client" +
                        "&response_type=code" +
                        "&redirect_uri=http://localhost:5002/home/auth" +
                        "&scope=customAPI+read+openid+profile" +
                        "&state={state}";

            return Redirect(url);
        }

        public async Task Auth(string code, string scope, string state)
        {
            var client = new HttpClient();

            var tokenRequest = new AuthorizationCodeTokenRequest
            {
                ClientId = "cf.client",
                ClientSecret = "secret",
                Address = $"{authserverurl}/connect/token",
                Code = code,
                RedirectUri = "http://localhost:5002/home/auth",
                GrantType = "authorization_code"
            };

            var credentialsResponse = client.RequestAuthorizationCodeTokenAsync(tokenRequest);

            if (credentialsResponse.Result.IsError)
            {
                Console.WriteLine(credentialsResponse.Result.Error);
                return;
            }

            client.SetBearerToken(credentialsResponse.Result.AccessToken);

            var response = await client.GetAsync("http://localhost:5003/api/values");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }

            GetUserInfo(credentialsResponse.Result.AccessToken);

            return;
        }

        public void GetUserInfo(string accessToken)
        {
            // in order to get access to additional token "profile" scope must be requested
            var infoRequest = new IdentityModel.Client.UserInfoRequest
            {
                Token = accessToken,
                Address = $"{authserverurl}/connect/userinfo"
            };

            var client = new HttpClient();
            var userinforesponse = client.GetUserInfoAsync(infoRequest);

            if (userinforesponse.Result.IsError)
            {
                Console.WriteLine(userinforesponse.Result.Error);
                return;
            }

        }

    }
}
