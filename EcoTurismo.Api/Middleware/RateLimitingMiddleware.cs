using System.Collections.Concurrent;

namespace EcoTurismo.Api.Middleware;

/// <summary>
/// Middleware simples de rate limiting por IP
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, (DateTime, int)> _requestCounts = new();
    private const int MAX_REQUESTS_PER_MINUTE = 60;
    private const int CLEANUP_INTERVAL_MINUTES = 10;
    private static DateTime _lastCleanup = DateTime.UtcNow;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Limpar entradas antigas periodicamente
        if ((DateTime.UtcNow - _lastCleanup).TotalMinutes > CLEANUP_INTERVAL_MINUTES)
        {
            CleanupOldEntries();
        }

        // Pegar IP do cliente
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Se está autenticado, pode ter limite maior ou sem limite
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        var now = DateTime.UtcNow;
        var key = $"{clientIp}_{now:yyyyMMddHHmm}"; // Chave por minuto

        var (timestamp, count) = _requestCounts.GetOrAdd(key, _ => (now, 0));

        // Se passou mais de 1 minuto, resetar contador
        if ((now - timestamp).TotalMinutes >= 1)
        {
            _requestCounts[key] = (now, 1);
            await _next(context);
            return;
        }

        // Incrementar contador
        count++;
        _requestCounts[key] = (timestamp, count);

        // Verificar limite
        if (count > MAX_REQUESTS_PER_MINUTE)
        {
            _logger.LogWarning("Rate limit excedido: {IP} - {Count} requisições", 
                clientIp, count);

            context.Response.StatusCode = 429; // Too Many Requests
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Retry-After", "60");
            
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                errorMessage = "Muitas requisições. Tente novamente em 1 minuto.",
                data = (object?)null
            }));
            return;
        }

        await _next(context);
    }

    private static void CleanupOldEntries()
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-5);
        var keysToRemove = _requestCounts
            .Where(kvp => kvp.Value.Item1 < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _requestCounts.TryRemove(key, out _);
        }

        _lastCleanup = DateTime.UtcNow;
    }
}
