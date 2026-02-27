using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class UpdateReservaStatusEndpoint : Endpoint<UpdateReservaStatusRequest>
{
    private readonly IReservaService _service;

    public UpdateReservaStatusEndpoint(IReservaService service) => _service = service;

    public override void Configure()
    {
        Put("/api/reservas/{Id}/status");
        Policies(RolePolicies.AdminOrPrefeituraPolicy); // Admin ou Prefeitura podem alterar status
    }

    public override async Task HandleAsync(UpdateReservaStatusRequest req, CancellationToken ct)
    {
        var ok = await _service.AtualizarStatusAsync(req.Id, req.Status);

        if (!ok)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
