using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Extensions;
using IdentityServer.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment Env;

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment _env
            )
        {
            Configuration = configuration;
            Env = _env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityConfig(Configuration);

            services.AddIdentityServerConfig(Configuration, Env);

            var secretsFacebook = Configuration.GetSection(ExternalSecretsModel.Facebook).Get<ExternalSecretsModel>();

            services.AddAuthentication()
                .AddFacebook(config =>
                {
                    config.AppId = secretsFacebook.Id;
                    config.AppSecret = secretsFacebook.Secret;
                    // config.CallbackPath = "https://localhost:44353/signin-facebook"; // IdentityServer4 it's set on the Facebook Config Page
                });

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
