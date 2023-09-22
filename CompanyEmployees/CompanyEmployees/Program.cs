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

            _ = app.MapControllers();

            app.Run();
        }
    }
}
