using CompanyEmployees.Extensions;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace CompanyEmployees
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            _ = LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

            // Add services to the container.

            builder.Services.ConfigureCors();

            builder.Services.ConfigureIISIntegration();

            builder.Services.ConfigureLoggerService();

            builder.Services.ConfigureRepositoryManager();

            builder.Services.ConfigureServiceManager();

            builder.Services.ConfigureSqlContext(builder.Configuration);

            _ = builder.Services.AddAutoMapper(typeof(Program));

            //// With this, we are suppressing a default model state validation that is implemented 
            //// due to the existence of the [ApiController] attribute in all API controllers.
            _ = builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            _ = builder.Services.AddControllers(configure =>
            {
                configure.RespectBrowserAcceptHeader = true;
                configure.ReturnHttpNotAcceptable = true;
            })
                .AddXmlDataContractSerializerFormatters()
                .AddCustomCSVFormatter()
                .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.

            ILoggerManager logger = app.Services.GetRequiredService<ILoggerManager>();

            app.ConfigureExceptionHandler(logger);

            ////if (app.Environment.IsDevelopment())
            ////{
            ////    _ = app.UseDeveloperExceptionPage();
            ////}
            if (app.Environment.IsProduction())
            {
                _ = app.UseHsts();////adds the Strict-Transport-Security header
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseStaticFiles();////enables using static files for the request

            _ = app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
            });//// will forward proxy headers to the current request

            _ = app.UseCors("CorsPolicy");

            _ = app.UseAuthorization();

            ////_ = app.Use(async (httpContext, requestDelegate) =>
            ////{
            ////    Console.WriteLine($"Logic before executing the next delegate in the Use method");
            ////    await requestDelegate.Invoke();
            ////    Console.WriteLine($"Logic after executing the next delegate in the Use method");
            ////});

            ////_ = app.Map("/usingmapbranch", builder =>
            ////{
            ////    _ = builder.Use(async (httpContext, requestDelegate) =>
            ////    {
            ////        Console.WriteLine("Map branch logic in the Use method before the next delegate");
            ////        await requestDelegate.Invoke();
            ////        Console.WriteLine("Map branch logic in the Use method after the next delegate");
            ////    });
            ////    builder.Run(async httpContext =>
            ////    {
            ////        Console.WriteLine($"Map branch response to the client in the Run method");
            ////        await httpContext.Response.WriteAsync("Hello from the map branch.");
            ////    });
            ////});

            ////_ = app.MapWhen(httpContext => httpContext.Request.Query.ContainsKey("testquerystring"), builder =>
            ////    {
            ////        builder.Run(async httpContext =>
            ////        {
            ////            if(httpContext.Request.Query.TryGetValue("testquerystring", out StringValues value))
            ////                await httpContext.Response.WriteAsync($"Hello from the MapWhen branch. {value}");
            ////        });
            ////    });

            ////app.Run(async httpContext =>
            ////{
            ////    Console.WriteLine($"Writing the response to the client in the Run method");
            ////    await httpContext.Response.WriteAsync("Hello from the middleware component.");
            ////});

            _ = app.MapControllers();

            app.Run();
        }
    }
}