// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CompanyEmployees.IDP;

public class Startup
{
	public IWebHostEnvironment Environment { get; }

	public Startup(IWebHostEnvironment environment)
	{
		Environment = environment;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		// uncomment, if you want to add an MVC-based UI
		services.AddControllersWithViews();

		var builder = services.AddIdentityServer(options =>
		{
				// see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
				options.EmitStaticAudienceClaim = true;
		})
			.AddInMemoryIdentityResources(Config.Ids)
			.AddInMemoryApiScopes(Config.ApiScopes)
			.AddInMemoryApiResources(Config.Apis)
			.AddInMemoryClients(Config.Clients)
			.AddTestUsers(TestUsers.Users);

		// not recommended for production - you need to store your key material somewhere secure
		builder.AddDeveloperSigningCredential();
	}

	public void Configure(IApplicationBuilder app)
	{
		if (Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseStaticFiles();
		app.UseRouting();

		app.UseIdentityServer();

		app.UseAuthorization();
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapDefaultControllerRoute();
		});
	}
}
