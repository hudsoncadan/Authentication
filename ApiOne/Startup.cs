using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ApiOne
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", config =>
                {
                    config.Authority = "https://localhost:44353/"; // IdentityServer4
                    config.Audience = "ApiOne";
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        // It's set to Zero for test purpose. Default value is 5 minutes
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            services.AddCors(config =>
                config.AddPolicy("AllowAll", p =>
                    p.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
