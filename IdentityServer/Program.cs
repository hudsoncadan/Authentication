using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Mock data
            using(var scope = host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var user = new IdentityUser("usertest");
                userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
                userManager.AddClaimAsync(user, new Claim("Product", "Read;Add;Update;")).GetAwaiter().GetResult();
                userManager.AddClaimAsync(user, new Claim("Vendor", "Read;")).GetAwaiter().GetResult();

                // Initializing the database
                // http://docs.identityserver.io/en/latest/quickstarts/5_entityframework.html#initializing-the-database
                scope.ServiceProvider
                    .GetRequiredService<PersistedGrantDbContext>()
                    .Database
                    .Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                context.Clients.RemoveRange(
                    context.Clients.ToListAsync().GetAwaiter().GetResult());
                context.SaveChanges();
                if (!context.Clients.Any())
                {
                    foreach (var client in Configuration.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                context.IdentityResources.RemoveRange(
                    context.IdentityResources.ToListAsync().GetAwaiter().GetResult());
                context.SaveChanges();
                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Configuration.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }


                context.ApiScopes.RemoveRange(
                  context.ApiScopes.ToListAsync().GetAwaiter().GetResult());
                context.SaveChanges();
                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Configuration.GetApiScopes())
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                context.ApiResources.RemoveRange(
                    context.ApiResources.ToListAsync().GetAwaiter().GetResult());
                context.SaveChanges();
                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Configuration.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
