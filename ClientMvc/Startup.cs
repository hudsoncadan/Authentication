using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClientMvc
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                config.DefaultScheme = "ClientMvc.Cookie";
                config.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("ClientMvc.Cookie")
                .AddOpenIdConnect("oidc", config =>
                {
                    config.Authority = "https://localhost:44353/"; // IdentityServer4
                    config.ClientId = "client_id_mvc";
                    config.ClientSecret = "client_secret_mvc";
                    config.SaveTokens = true;

                    // The actual flow it is going to use
                    // https://openid.net/specs/openid-connect-core-1_0.html#Authentication
                    config.ResponseType = "code";

                    // Configure cookie claim mapping
                    config.ClaimActions.DeleteClaim("amr");
                    config.ClaimActions.DeleteClaim("s_hash");
                    config.ClaimActions.MapUniqueJsonKey("Produto", "Produto");

                    // Configure scopes
                    config.Scope.Clear();
                    config.Scope.Add("openid");
                    config.Scope.Add("userPermissionsScope");
                    config.Scope.Add("ApiOne");
                    config.Scope.Add("offline_access"); // https://openid.net/specs/openid-connect-core-1_0.html#OfflineAccess


                    // Just in case you want to keep the jwt token smaller
                    // Two trips to load claims into the cookie
                    //
                    // if it's set to false, you may set your identityserver's client to AlwaysIncludeUserClaimsInIdToken = false,
                    config.GetClaimsFromUserInfoEndpoint = true;

                    config.SignedOutCallbackPath = "/Home/Index";

                    config.UsePkce = true;
                });

            services.AddHttpClient();

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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
