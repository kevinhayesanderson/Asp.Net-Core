using Contracts;
using Entities.ErrorModel;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace CompanyEmployees.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager logger)
        {
            ////This method adds a middleware to the pipeline that will catch exceptions, 
            ////log them, and re-execute the request in an alternate pipeline.
            app.UseExceptionHandler(appBuilder =>
            {
                ////Run method, which adds a terminal middleware delegate to the application’s pipeline.
                appBuilder.Run(async httpContext =>
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    httpContext.Response.ContentType = "application/json";

                    var contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await httpContext.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = httpContext.Response.StatusCode,
                            Message = "Internal Server Error."
                        }.ToString());
                    }
                });
            });
        }
    }
}
