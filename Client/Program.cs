using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();


        private static async Task MainAsync()
        {

            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            //var tokenResponse = await ResourceOwnerPasswordFlow(disco.TokenEndpoint, disco.AuthorizeEndpoint);
            var tokenResponse = await ClientCredentialsFlow(disco.TokenEndpoint, disco.AuthorizeEndpoint);

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }


            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.WriteLine("Finished...");
            Console.ReadLine();
        }

        private static async Task<TokenResponse> ResourceOwnerPasswordFlow(string tokenEndpoint, string authorizationEndpoint)
        {

            var tokenClient = new TokenClient(tokenEndpoint, "ro.client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "mypass", "customAPI");

            return tokenResponse;
        }

        private static async Task<TokenResponse> ClientCredentialsFlow(string tokenEndpoint, string authorizationEndpoint)
        {

            var tokenClient = new TokenClient(tokenEndpoint, "cc.client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("customAPI");

            return tokenResponse;
        }

    }
}
