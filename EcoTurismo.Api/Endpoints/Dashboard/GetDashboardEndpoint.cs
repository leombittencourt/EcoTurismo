using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Dashboard;

public class GetDashboardEndpoint : Endpoint<GetDashboardRequest, DashboardDto>
{
    private readonly IDashboardService _service;

    public GetDashboardEndpoint(IDashboardService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("/api/dashboard");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        Description(d => d
            .WithTags("Dashboard")
            .WithSummary("Obtém dados do dashboard")
            .WithDescription("Retorna estatísticas e métricas do sistema para o período especificado")
            .Produces<DashboardDto>(200)
            .Produces(400));
    }

    public override async Task HandleAsync(GetDashboardRequest req, CancellationToken ct)
    {
        var dashboard = await _service.GetDashboardAsync(req.Periodo, ct);
        await Send.OkAsync(dashboard, ct);
    }
}
