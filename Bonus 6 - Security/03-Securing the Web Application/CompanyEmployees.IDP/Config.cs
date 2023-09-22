using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace CompanyEmployees.IDP;

public static class Config
{
	public static IEnumerable<IdentityResource> Ids =>
	new IdentityResource[]
	{
			new IdentityResources.OpenId(),
			new IdentityResources.Profile()
	};

	public static IEnumerable<ApiScope> ApiScopes =>
	new ApiScope[]
	{ };

	public static IEnumerable<ApiResource> Apis =>
	new ApiResource[]
	{ };

	public static IEnumerable<Client> Clients =>
	new Client[]
	{
			new Client
			{
				ClientName = "CompanyEmployeeClient",
				ClientId = "companyemployeeclient",
				AllowedGrantTypes = GrantTypes.Code,
				RedirectUris = new List<string>{ "https://localhost:5010/signin-oidc" },
				AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile },
				ClientSecrets = { new Secret("CompanyEmployeeClientSecret".Sha512()) },
				RequirePkce = true,
				PostLogoutRedirectUris = new List<string> { "https://localhost:5010/signout-callback-oidc" },
				RequireConsent = true,
				ClientUri = "https://localhost:5010"
			}
	};
}
