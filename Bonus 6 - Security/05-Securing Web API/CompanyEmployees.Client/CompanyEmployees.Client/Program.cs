using CompanyEmployees.Client.Handlers;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddHttpClient("APIClient", client =>
{
	client.BaseAddress = new Uri("https://localhost:5001/");
	client.DefaultRequestHeaders.Clear();
	client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient("IDPClient", client =>
{
	client.BaseAddress = new Uri("https://localhost:5005/");
	client.DefaultRequestHeaders.Clear();
	client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});

builder.Services.AddAuthentication(opt =>
{
	opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
{
	opt.AccessDeniedPath = "/Auth/AccessDenied";
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opt =>
{
	opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	opt.Authority = "https://localhost:5005";
	opt.ClientId = "companyemployeeclient";
	opt.ResponseType = OpenIdConnectResponseType.Code;
	opt.SaveTokens = true;
	opt.ClientSecret = "CompanyEmployeeClientSecret";
	opt.GetClaimsFromUserInfoEndpoint = true;
	opt.ClaimActions.DeleteClaim("sid");
	opt.ClaimActions.DeleteClaim("idp");
	opt.Scope.Add("address");
	opt.Scope.Add("roles");
	opt.ClaimActions.MapUniqueJsonKey("role", "role");
	opt.Scope.Add("companyemployeeapi.scope");
	opt.Scope.Add("country");
	opt.ClaimActions.MapUniqueJsonKey("country", "country");


	opt.TokenValidationParameters = new TokenValidationParameters
	{
		RoleClaimType = JwtClaimTypes.Role
	};
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<BearerTokenHandler>();

builder.Services.AddAuthorization(authOpt =>
{
	authOpt.AddPolicy("CanCreateAndModifyData", policyBuilder =>
	{
		policyBuilder.RequireAuthenticatedUser();
		policyBuilder.RequireRole("role", "Administrator");
		policyBuilder.RequireClaim("country", "USA");
	});
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
