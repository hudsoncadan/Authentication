using IdentityServer.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer.Extensions
{
    public static class IdentityServerConfig
    {
        public static IServiceCollection AddIdentityServerConfig(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            // Certificate
            // https://docs.microsoft.com/en-us/archive/blogs/kaevans/using-powershell-with-certificates
            var certificatePath = Path.Combine(env.ContentRootPath, 
                configuration.GetSection(CertificateModel.Certificate)
                             .Get<CertificateModel>()
                             .Name);

            var certificate = new X509Certificate2(certificatePath, 
                configuration.GetSection(CertificateModel.Certificate)
                             .Get<CertificateModel>()
                             .Password);

            // OpenID Provider's configuration
            // .well -known/openid-configuration
            services.AddIdentityServer()
                .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
                .AddInMemoryApiResources(Configuration.GetApiResources())
                .AddInMemoryApiScopes(Configuration.GetApiScopes())
                .AddInMemoryClients(Configuration.GetClients())

                .AddAspNetIdentity<IdentityUser>()

                .AddSigningCredential(certificate);

            return services;
        }
    }
}
