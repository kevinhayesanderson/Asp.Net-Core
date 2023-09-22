// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using CompanyEmployees.IDP.CustomTokenProviders;
using CompanyEmployees.IDP.Entities;
using EmailService;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace CompanyEmployees.IDP;

public class Startup
{
	public IWebHostEnvironment Environment { get; }
	public IConfiguration Configuration { get; set; }

	public Startup(IWebHostEnvironment environment, IConfiguration configuration)
	{
		Environment = environment;
		Configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddAutoMapper(typeof(Startup));

		var emailConfig = Configuration
			.GetSection("EmailConfiguration")
			.Get<EmailConfiguration>();
		services.AddSingleton(emailConfig);
		services.AddScoped<IEmailSender, EmailSender>();

		services.AddControllersWithViews();

		var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

		services.AddDbContext<UserContext>(options => options
			.UseSqlServer(Configuration.GetConnectionString("identitySqlConnection")));

		services.AddIdentity<User, IdentityRole>(opt =>
		{
			opt.Password.RequireDigit = false;
			opt.Password.RequiredLength = 7;
			opt.Password.RequireUppercase = false;
			opt.User.RequireUniqueEmail = true;
			opt.SignIn.RequireConfirmedEmail = true;
			opt.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
			opt.Lockout.AllowedForNewUsers = true;
			opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
			opt.Lockout.MaxFailedAccessAttempts = 3;
		})
		.AddEntityFrameworkStores<UserContext>()
		.AddDefaultTokenProviders()
		.AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation");


		var builder = services.AddIdentityServer(options =>
		{
			options.EmitStaticAudienceClaim = true;
		})
		.AddConfigurationStore(opt =>
		{
			opt.ConfigureDbContext = c => c.UseSqlServer(Configuration.GetConnectionString("sqlConnection"),
			sql => sql.MigrationsAssembly(migrationAssembly));
		})
		.AddOperationalStore(opt =>
		{
			opt.ConfigureDbContext = o => o.UseSqlServer(Configuration.GetConnectionString("sqlConnection"),
			sql => sql.MigrationsAssembly(migrationAssembly));
		})
		.AddAspNetIdentity<User>();

		builder.AddDeveloperSigningCredential();

		services.Configure<DataProtectionTokenProviderOptions>(opt =>
			opt.TokenLifespan = TimeSpan.FromHours(2));

		services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
			opt.TokenLifespan = TimeSpan.FromDays(3));

		services.AddAuthentication()
		.AddGoogle(options =>
		{
			options.ClientId = "979797444470-3761h1tao21ijeg85gol43usip6kmbov.apps.googleusercontent.com";
			options.ClientSecret = "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";
		});
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
