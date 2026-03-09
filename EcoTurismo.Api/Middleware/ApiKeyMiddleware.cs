namespace EcoTurismo.Api.Middleware;

/// <summary>
/// Middleware para validar API Key em requisiÃ§Ãµes pÃºblicas
/// Serve como primeira camada de proteÃ§Ã£o contra uso nÃ£o autorizado
/// </summary>
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private const string API_KEY_HEADER = "X-API-Key";

    public ApiKeyMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Preflight de CORS nunca deve ser bloqueado por API Key.
        if (HttpMethods.IsOptions(context.Request.Method))
        {
            await _next(context);
            return;
        }

        // Lista de endpoints que requerem API Key (pÃºblicos mas protegidos)
        var protectedPublicPaths = new[]
        {
            "/api/quiosques",
            "/api/configuracoes",
            "/api/municipios",
            "/api/banners"
        };

        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        
        // Se nÃ£o Ã© um path protegido, continua normalmente
        if (!protectedPublicPaths.Any(p => path.StartsWith(p)))
        {
            await _next(context);
            return;
        }

        // Se jÃ¡ estÃ¡ autenticado (tem token JWT), nÃ£o precisa API Key
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        // Verificar API Key
        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
        {
            _logger.LogWarning("Tentativa de acesso sem API Key: {Path} de {IP}", 
                path, context.Connection.RemoteIpAddress);
            
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                errorMessage = "API Key Ã© obrigatÃ³ria para acessar este recurso.",
                data = (object?)null
            }));
            return;
        }

        var validApiKeys = _configuration.GetSection("ApiKeys:ValidKeys").Get<string[]>() ?? Array.Empty<string>();

        if (!validApiKeys.Any(k => k == extractedApiKey))
        {
            _logger.LogWarning("API Key invÃ¡lida: {Path} de {IP}", 
                path, context.Connection.RemoteIpAddress);
            
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                errorMessage = "API Key invÃ¡lida.",
                data = (object?)null
            }));
            return;
        }

        // API Key vÃ¡lida, continuar
        await _next(context);
    }
}
