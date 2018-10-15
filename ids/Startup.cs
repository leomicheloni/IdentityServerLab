﻿using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ids
{
    public class Startup
    {
        private string clientId = "44b2da50-090d-4d62-b4dc-5156eeecf7a0";
        private string clientSecret = "fZDkC60ghULrH8sQyziFZN1oeNNZpGT+oUIpuLdcfiE=";
        private string authority = "https://login.microsoftonline.com/abc9b1b9-75e9-41e6-a5cf-d58c753ecc7b/";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

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
                options.Authority = authority;
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.SaveTokens = true;

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
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
            app.UseCors("default");
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
