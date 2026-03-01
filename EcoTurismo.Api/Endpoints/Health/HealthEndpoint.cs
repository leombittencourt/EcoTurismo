using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcoTurismo.Api.Endpoints.Health;

public class HealthEndpoint : EndpointWithoutRequest<HealthResponse>
{
    private readonly EcoTurismoDbContext _db;
    private readonly IConfiguration _configuration;
    private static readonly DateTimeOffset _startTime = DateTimeOffset.UtcNow;

    public HealthEndpoint(EcoTurismoDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public override void Configure()
    {
        Get("/health");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new HealthResponse
        {
            Status = "healthy",
            Version = GetVersion(),
            Timestamp = DateTimeOffset.UtcNow,
            UptimeSeconds = (long)(DateTimeOffset.UtcNow - _startTime).TotalSeconds,
            Checks = new HealthChecks
            {
                Api = true,
                Database = await CheckDatabaseAsync(ct),
                Environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production"
            }
        };

        // Se algum check falhar, marca como unhealthy
        if (!response.Checks.Database)
        {
            response.Status = "unhealthy";
            HttpContext.Response.StatusCode = 503;
        }

        await Send.OkAsync(response, ct);
    }

    private async Task<bool> CheckDatabaseAsync(CancellationToken ct)
    {
        try
        {
            // Tenta fazer uma query simples para verificar conexão
            await _db.Database.ExecuteSqlRawAsync("SELECT 1", ct);
            return true;
        }
        catch (Exception ex)
        {
            // Log o erro mas não expõe detalhes
            Console.WriteLine($"❌ Health Check - Database falhou: {ex.Message}");
            return false;
        }
    }

    private string GetVersion()
    {
        try
        {
            var assembly = typeof(Program).Assembly;
            var version = assembly.GetName().Version;
            return version?.ToString() ?? "1.0.0";
        }
        catch
        {
            return "1.0.0";
        }
    }
}
