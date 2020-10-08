using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullStackJobs.AuthServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("resourceapi", "Resource API")
                {
                    // The 'new Scope("api.read")' breaks as it seems there is no interface to a new Scope from within the ApiResource.
                    Scopes = { "api.read" }
                }
            };
        }

        public static IEnumerable<Client> GetClients(string devHost = "")
        {
            return new[]
            {
                new Client
                {
                    RequireConsent = false,
                    ClientId = "js_test_client",
                    ClientName = "JavaScript Test Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    AllowedScopes = { "openid", "profile", "email", "api.read" },
                    RedirectUris = { $"http://{devHost}/test-client/callback.html" }, // This could be configured further up chain.
                    AllowedCorsOrigins = { $"http://{devHost}" }, // This could be configured further up chain.
                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(120).TotalSeconds
                },
                new Client
                {
                    RequireConsent = false,
                    ClientId = "angular_spa",
                    ClientName = "Angular Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    AllowedScopes = { "openid", "profile", "email", "api.read" },
                    RedirectUris = { "http://localhost:4200/auth-callback.html" }, // This could be configured in the JSON configuration file.
                    PostLogoutRedirectUris = new List<string> { "http://localhost:4200/" }, // This could be configured in the JSON configuration file.
                    AllowedCorsOrigins = { "http://localhost:4200" }, // This could be configured.
                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(120).TotalSeconds
                }
            };
        }
    }
}
