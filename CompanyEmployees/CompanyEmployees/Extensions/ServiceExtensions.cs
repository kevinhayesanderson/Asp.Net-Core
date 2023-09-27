// Ignore Spelling: Cors

using Contracts;
using LoggerService;
using Repository;

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

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            _ = services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            _ = services.AddScoped<IRepositoryManager, RepositoryManager>();
        }
    }
}
