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
	{ };
}
