namespace EcoTurismo.Api.Middleware;

/// <summary>
/// Middleware para validar API Key em requisições públicas
/// Serve como primeira camada de proteção contra uso não autorizado
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
        // Lista de endpoints que requerem API Key (públicos mas protegidos)
        var protectedPublicPaths = new[]
        {
            "/api/quiosques",
            "/api/configuracoes",
            "/api/municipios",
            "/api/banners"
        };

        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        
        // Se não é um path protegido, continua normalmente
        if (!protectedPublicPaths.Any(p => path.StartsWith(p)))
        {
            await _next(context);
            return;
        }

        // Se já está autenticado (tem token JWT), não precisa API Key
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
                errorMessage = "API Key é obrigatória para acessar este recurso.",
                data = (object?)null
            }));
            return;
        }

        var validApiKeys = _configuration.GetSection("ApiKeys:ValidKeys").Get<string[]>() ?? Array.Empty<string>();

        if (!validApiKeys.Any(k => k == extractedApiKey))
        {
            _logger.LogWarning("API Key inválida: {Path} de {IP}", 
                path, context.Connection.RemoteIpAddress);
            
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                errorMessage = "API Key inválida.",
                data = (object?)null
            }));
            return;
        }

        // API Key válida, continuar
        await _next(context);
    }
}
