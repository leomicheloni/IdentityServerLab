using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ids
{
    public class Startup
    {
        string clientId = "";
        string clientSecret = "";
        string authority = "";


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers())
                .AddInMemoryIdentityResources(Config.GetIdentityResources());

            services.AddAuthentication().AddOpenIdConnect("oidc", "Open id connect", options =>
            {
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.ClaimActions.Clear();
                options.Authority = authority;
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.GetClaimsFromUserInfoEndpoint = true;
                
                options.Events = new OpenIdConnectEvents()
                {
                    OnUserInformationReceived = (context) =>
                    {
                        ClaimsIdentity claimsId = context.Principal.Identity as ClaimsIdentity;

                        var roles = context.User.Children().FirstOrDefault(j => j.Path == JwtClaimTypes.Role).Values().ToList();
                        claimsId.AddClaims(roles.Select(r => new Claim(JwtClaimTypes.Role, r.Value<String>())));
                        return Task.FromResult(0);
                    }
                };

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
