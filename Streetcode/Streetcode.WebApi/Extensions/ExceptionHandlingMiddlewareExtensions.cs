using Streetcode.WebApi.Middleware.ExceptionHandlingMiddleware;

namespace Streetcode.WebApi.Extensions;

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void AddExceptionHandlingMiddlewareToServices(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();
    }
}