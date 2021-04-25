using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extensions
{
    public static class IdentityServerConfig
    {
        public static IServiceCollection AddIdentityServerConfig(this IServiceCollection services)
        {
            // OpenID Provider's configuration
            // .well -known/openid-configuration
            services.AddIdentityServer()
                .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
                .AddInMemoryApiResources(Configuration.GetApiResources())
                .AddInMemoryApiScopes(Configuration.GetApiScopes())
                .AddInMemoryClients(Configuration.GetClients())

                .AddAspNetIdentity<IdentityUser>()

                .AddDeveloperSigningCredential();

            return services;
        }
    }
}
