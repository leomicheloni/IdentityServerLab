using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace ids
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("customAPI", "Custom Web API"),
                new ApiResource("api1", "Otra aPI"),
                new ApiResource("read", "read rights"),
                new ApiResource("write", "write rigths"),
            };
        }
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser> {
                new TestUser
                {
                    Password = "mypass",
                    Username = "alice",
                    SubjectId = "alice@alice.com"
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "ro.client",
                    ClientName = "Resource owner password",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "customAPI"
                    },
                    ClientSecrets = { new Secret ( "secret".Sha256())}
                },
                new Client
                {
                    ClientId = "angular",
                    ClientName = "CustomClient",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = { "http://localhost:4200/auth-callback" },
                    PostLogoutRedirectUris = { "http://localhost:4200/" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "customAPI"
                    },
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequireConsent = false,
                    EnableLocalLogin = false
                },
                new Client
                {
                    ClientId = "cc.client",
                    ClientName = "Client credentials",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret ( "secret".Sha256())},

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "customAPI"
                    }
                },
                new Client
                {
                    ClientId = "cf.client",
                    ClientName = "Authorization Code Client",
                    ClientSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:5002/home/auth"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:5002/home"
                    },
                    AllowedScopes = new List<string>
                    {
                            "customAPI",
                            "read",
                            "write"
                    },
                    AccessTokenType = AccessTokenType.Jwt
                }
            };
        }
    }
}
