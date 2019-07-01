﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace API.Profiles
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                //var policy = ScopePolicy.Create("customAPI"); //policy filter applies to controllers and is verified against client scope
                //options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddAuthorization(options =>
            {
                options.AddPolicy("ALL", builder => // this policies apply at action level and are composed of multiple scopes
                {
                    builder.RequireScope("read");
                    builder.RequireScope("write");
                });
                options.AddPolicy("Read", builder =>
                {
                    builder.RequireScope("read");
                });
            })
            .AddJsonFormatters();

            services.AddAuthentication("Bearer")

            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.ApiName = "customAPI";
                options.EnableCaching = true;
                options.CacheDuration = TimeSpan.FromDays(30);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
