using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Configuration
    {
        // These claims are added to the Profile response 
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "userPermissionsScope",
                    UserClaims =
                    {
                        "Produto"
                    }
                }
            };

        // v4 - These claims are added to access_token
        public static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope> {
                new ApiScope("ApiOne", new string[]{ "Fornecedor", "Produto" }),
            };

        // Scopes ApiOne allows to use JWT in ApiOne Authentication
        public static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource> {
                 new ApiResource("ApiOne")
                {
                    Scopes = new []{ "ApiOne" }
                },
            };

        public static IEnumerable<Client> GetClients()
        {
            var resource_api_one = "ApiOne";
            var client_url_mvc = "https://localhost:44325";
            var client_url_js = "https://localhost:44307";

            return new List<Client>
            {
                new Client
                {
                    ClientId = "client_id_mvc",
                    ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.Code, // the way we can retrieve the access token
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        resource_api_one,
                        "userPermissionsScope",
                    },
                    RedirectUris = { $"{client_url_mvc}/signin-oidc" }, // ClientMVC
                   
                    RequirePkce = true,
                    RequireConsent = false,

                    PostLogoutRedirectUris = { $"{client_url_mvc}/Home/Index" }, // ClientMVC

                    // If it's set to true, adds all the User's claims into the id_token
                    // If it's set to false, you can keep the id_token smaller
                    // Then you must set your client's startup to config.GetClaimsFromUserInfoEndpoint = true;
                    AlwaysIncludeUserClaimsInIdToken = false,

                    // RefreshToken https://openid.net/specs/openid-connect-core-1_0.html#OfflineAccess
                    AllowOfflineAccess = true,
                },
                new Client
                {
                    // This config returns id_token and access_token in the url
                    // RequirePkce = false,
                    // AllowedGrantTypes = GrantTypes.Implicit, // the way we can retrieve the access token

                    // GrandType.Code requires client_secret by default, so we set it to false
                    AllowedGrantTypes = GrantTypes.Code, // the way we can retrieve the access token
                    RequireClientSecret = false,
                    RequirePkce = true,

                      ClientId = "client_id_js",
                      AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        resource_api_one,
                        "userPermissionsScope"
                    },
                    AllowedCorsOrigins = { client_url_js }, // ClientJS
                    
                    RedirectUris = { $"{client_url_js}/Home/SignIn" }, // ClientJS
                    RequireConsent = false,

                    PostLogoutRedirectUris = { $"{client_url_js}/Home/Index" }, // ClientJS

                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 2, // 2 seconds for test purpose
                    IdentityTokenLifetime = 2, // Must set TokenValidationParameters ClockSkew to Zero. The default value is 5 minutes.
                }
            };
        }
    }
}
