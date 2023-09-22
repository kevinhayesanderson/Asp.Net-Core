using CompanyEmployees.Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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

	public async Task<IActionResult> Companies()
	{
		var httpClient = _httpClientFactory.CreateClient("APIClient");

		var response = await httpClient.GetAsync("api/companies").ConfigureAwait(false);

		response.EnsureSuccessStatusCode();

		var companiesString = await response.Content.ReadAsStringAsync();
		var companies = JsonSerializer.Deserialize<List<CompanyViewModel>>(companiesString,
			new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

		return View(companies);
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
