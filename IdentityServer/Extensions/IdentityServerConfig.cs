using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IdentityServer.Extensions
{
    public static class IdentityServerConfig
    {
        public static IServiceCollection AddIdentityServerConfig(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name; 
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // OpenID Provider's configuration
            // .well -known/openid-configuration
            services.AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })

                .AddAspNetIdentity<IdentityUser>()

                .AddDeveloperSigningCredential();

            return services;
        }
    }
}
