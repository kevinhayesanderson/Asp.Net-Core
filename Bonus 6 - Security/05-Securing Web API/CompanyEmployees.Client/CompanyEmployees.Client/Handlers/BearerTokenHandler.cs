using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace CompanyEmployees.Client.Handlers;

public class BearerTokenHandler : DelegatingHandler
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

		if (!string.IsNullOrWhiteSpace(accessToken))
			request.SetBearerToken(accessToken);

		return await base.SendAsync(request, cancellationToken);
	}
}
