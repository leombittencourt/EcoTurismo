using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Services.Interfaces;
using EcoTurismo.Api.Services;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Geocode;

public class GeocodeEndpoint : EndpointWithoutRequest<List<GeocodeResultDto>>
{
    private readonly IGeocodingService _geocodingService;
    private readonly RateLimitingService _rateLimitingService;

    public GeocodeEndpoint(IGeocodingService geocodingService, RateLimitingService rateLimitingService)
    {
        _geocodingService = geocodingService;
        _rateLimitingService = rateLimitingService;
    }

    public override void Configure()
    {
        Get("/api/geocode");
        AllowAnonymous();
        Description(d => d
            .WithTags("Geocoding")
            .WithSummary("Busca localizações usando Google Maps")
            .WithDescription("Retorna uma lista de até 5 sugestões de localizações. Rate limit: 10 req/min por IP.")
            .Produces<List<GeocodeResultDto>>(200)
            .Produces(400)
            .Produces(429)
            .Produces(500));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = Query<string>("query");

        if (string.IsNullOrWhiteSpace(query))
        {
            ThrowError("O parâmetro 'query' é obrigatório");
            return;
        }

        if (query.Length < 4)
        {
            ThrowError("O parâmetro 'query' deve ter pelo menos 4 caracteres");
            return;
        }

        // Rate limiting por IP
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var rateLimitKey = $"geocode:{clientIp}";

        if (!_rateLimitingService.IsAllowed(rateLimitKey))
        {
            var (remaining, resetAt) = _rateLimitingService.GetLimitInfo(rateLimitKey);

            HttpContext.Response.StatusCode = 429;
            HttpContext.Response.Headers.Append("X-RateLimit-Limit", "10");
            HttpContext.Response.Headers.Append("X-RateLimit-Remaining", remaining.ToString());
            HttpContext.Response.Headers.Append("X-RateLimit-Reset", resetAt.ToUnixTimeSeconds().ToString());

            await HttpContext.Response.WriteAsJsonAsync(new 
            { 
                error = "Limite de requisições excedido. Aguarde alguns segundos.",
                retryAfter = (int)(resetAt - DateTimeOffset.UtcNow).TotalSeconds
            }, ct);
            return;
        }

        try
        {
            var results = await _geocodingService.SearchAsync(query, ct);

            // Adicionar headers de rate limit
            var (remaining, resetAt) = _rateLimitingService.GetLimitInfo(rateLimitKey);
            HttpContext.Response.Headers.Append("X-RateLimit-Limit", "10");
            HttpContext.Response.Headers.Append("X-RateLimit-Remaining", remaining.ToString());
            HttpContext.Response.Headers.Append("X-RateLimit-Reset", resetAt.ToUnixTimeSeconds().ToString());

            await Send.OkAsync(results, ct);
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message);
        }
    }
}
