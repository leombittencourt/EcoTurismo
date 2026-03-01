using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Health;

/// <summary>
/// Readiness Probe - Verifica se a aplicação está pronta para receber tráfego
/// </summary>
public class ReadyEndpoint : EndpointWithoutRequest
{
    private readonly EcoTurismoDbContext _db;

    public ReadyEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/health/ready");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            // Verifica se consegue conectar no banco
            await _db.Database.ExecuteSqlRawAsync("SELECT 1", ct);

            await Send.OkAsync(new { status = "ready" }, ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Readiness Check falhou: {ex.Message}");
            HttpContext.Response.StatusCode = 503;
            await Send.OkAsync(new 
            { 
                status = "not_ready",
                reason = "database_unavailable"
            }, ct);
        }
    }
}

/// <summary>
/// Liveness Probe - Verifica se a aplicação está viva (responde)
/// </summary>
public class LiveEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/health/live");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Endpoint muito simples - apenas verifica se a app está respondendo
        await Send.OkAsync(new { status = "alive" }, ct);
    }
}
