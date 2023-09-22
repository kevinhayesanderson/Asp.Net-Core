using CompanyEmployees.Client.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace CompanyEmployees.Client.Controllers;

public class HomeController : Controller
{
	private readonly IHttpClientFactory _httpClientFactory;

	public HomeController(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public IActionResult Index()
	{
		return View();
	}

	[Authorize]
	public async Task<IActionResult> Companies()
	{
		var httpClient = _httpClientFactory.CreateClient("APIClient");

		var response = await httpClient.GetAsync("api/companies").ConfigureAwait(false);

		if (response.IsSuccessStatusCode)
		{
			var companiesString = await response.Content.ReadAsStringAsync();
			var companies = JsonSerializer.Deserialize<List<CompanyViewModel>>(companiesString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			return View(companies);
		}
		else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
		{
			return RedirectToAction("AccessDenied", "Auth");
		}

		throw new Exception("There is a problem accessing the API.");
	}

	[Authorize(Policy = "CanCreateAndModifyData")]
	public async Task<IActionResult> Privacy()
	{
		var idpClient = _httpClientFactory.CreateClient("IDPClient");
		var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();

		var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

		var response = await idpClient.GetUserInfoAsync(new UserInfoRequest
		{
			Address = metaDataResponse.UserInfoEndpoint,
			Token = accessToken
		});

		if (response.IsError)
		{
			throw new Exception("Problem while fetching data from the UserInfo endpoint", response.Exception);
		}

		var addressClaim = response.Claims.FirstOrDefault(c => c.Type.Equals("address"));

		User.AddIdentity(new ClaimsIdentity(new List<Claim> { new Claim(addressClaim.Type.ToString(), addressClaim.Value.ToString()) }));

		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
