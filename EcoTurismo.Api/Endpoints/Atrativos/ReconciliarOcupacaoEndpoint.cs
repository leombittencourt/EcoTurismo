using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class ReconciliarOcupacaoEndpoint : EndpointWithoutRequest
{
    private readonly IOcupacaoService _ocupacaoService;

    public ReconciliarOcupacaoEndpoint(IOcupacaoService ocupacaoService)
    {
        _ocupacaoService = ocupacaoService;
    }

    public override void Configure()
    {
        Post("/api/atrativos/reconciliar-ocupacao");
        Policies(RolePolicies.AdminPolicy); // Apenas admin
        Description(d => d
            .WithTags("Atrativos")
            .WithSummary("Reconcilia ocupação de todos os atrativos")
            .WithDescription("Recalcula a ocupação atual de todos os atrativos baseado nas reservas ativas")
            .Produces(200)
            .Produces(401)
            .Produces(403));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await _ocupacaoService.ReconciliarOcupacoesAsync(ct);

        await Send.OkAsync(new
        {
            success = true,
            message = "Reconciliação de ocupações concluída com sucesso"
        }, ct);
    }
}
