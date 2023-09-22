using CompanyEmployees.Extensions;

namespace CompanyEmployees
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.ConfigureCors();

            builder.Services.ConfigureIISIntegration();

            _ = builder.Services.AddControllers();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
            }
            else
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

            app.Use(async (httpContext, requestDelegate) =>
            {
                Console.WriteLine($"Logic before executing the next delegate in the Use method");
                await requestDelegate.Invoke();
                Console.WriteLine($"Logic after executing the next delegate in the Use method");
            });
            
            app.Run(async httpContext =>
            {
                Console.WriteLine($"Writing the response to the client in the Run method");
                await httpContext.Response.WriteAsync("Hello from the middleware component.");
            });

            _ = app.MapControllers();

            app.Run();
        }
    }
}
