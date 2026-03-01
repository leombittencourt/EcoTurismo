namespace EcoTurismo.Api.Middleware;

public static class ApiKeyMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyValidation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiKeyMiddleware>();
    }
}
