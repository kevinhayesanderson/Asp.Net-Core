// Ignore Spelling: Cors

using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;

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
                    .AllowAnyHeader()  ////.WithHeaders("accept", "content-type");
                    .WithExposedHeaders("X-Pagination");
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

        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            _ = services.AddScoped<IServiceManager, ServiceManager>();
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            ////_ = services.AddDbContext<RepositoryContext>(optionsAction =>
            ////    optionsAction.UseSqlServer(configuration.GetConnectionString("sqlConnection"))); //old method
            _ = services.AddSqlServer<RepositoryContext>(configuration.GetConnectionString("sqlConnection"));
        }

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder)
        {
            return builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));
        }

        public static void AddCustomMediaTypes(this IServiceCollection services) 
        { 
            services.Configure<MvcOptions>(config => 
            { 
                var systemTextJsonOutputFormatter = config.OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()
                ?.FirstOrDefault(); 
                systemTextJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.kevin.hateoas+json"); 
                
                var xmlOutputFormatter = config.OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()
                ?.FirstOrDefault(); 
                xmlOutputFormatter?.SupportedMediaTypes.Add("application/vnd.kevin.hateoas+xml"); 
            }); 
        }
    }
}