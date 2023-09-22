// Ignore Spelling: Cors

namespace CompanyEmployees.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            _ = services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("CorsPolicy", corsPolicyBuilder =>
                {
                    _ = corsPolicyBuilder
                    .AllowAnyOrigin()   ////.WithOrigins("https://example.com")
                    .AllowAnyMethod()   ////.WithMethods("POST", "GET")
                    .AllowAnyHeader();  ////.WithHeaders("accept", "content-type");
                });
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            _ = services.Configure<IISOptions>(configureOptions =>
            {

            });
        }
    }
}
