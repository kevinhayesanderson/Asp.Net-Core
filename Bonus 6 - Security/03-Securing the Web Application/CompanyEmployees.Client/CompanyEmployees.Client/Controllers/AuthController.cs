using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Client.Controllers;

public class AuthController : Controller
{
	public IActionResult Login()
	{
		return Challenge(new AuthenticationProperties
		{
			RedirectUri = "/"
		});
	}

	public async Task Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
	}
}
