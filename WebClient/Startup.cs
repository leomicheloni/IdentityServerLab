using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace WebClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";

                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;

                options.ClientId = "mvc";
                options.SaveTokens = true;
            });

            //AddAuthentication adds the authentication services to DI.We are using a cookie as the primary means to authenticate a user(via "Cookies" as the DefaultScheme). We set the DefaultChallengeScheme to "oidc" because when we need the user to login, we will be using the OpenID Connect scheme.
            //We then use AddCookie to add the handler that can process cookies.
            //Finally, AddOpenIdConnect is used to configure the handler that perform the OpenID Connect protocol.The Authority indicates that we are trusting IdentityServer. We then identify this client via the ClientId. SignInScheme is used to issue a cookie using the cookie handler once the OpenID Connect protocol is complete.And SaveTokens is used to persist the tokens from IdentityServer in the cookie(as they will be needed later).
            //As well, we’ve turned off the JWT claim type mapping to allow well - known claims(e.g. ‘sub’ and ‘idp’) to flow through unmolested:

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
