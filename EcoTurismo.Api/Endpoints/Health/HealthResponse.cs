using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Health;

public class HealthResponse
{
    public string Status { get; set; } = "healthy";
    public string Version { get; set; } = "1.0.0";
    public DateTimeOffset Timestamp { get; set; }
    public long UptimeSeconds { get; set; }
    public HealthChecks Checks { get; set; } = new();
}

public class HealthChecks
{
    public bool Database { get; set; }
    public bool Api { get; set; }
    public string Environment { get; set; } = string.Empty;
}
