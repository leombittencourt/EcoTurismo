using EcoTurismo.Application.Interfaces;
using FastEndpoints;
using AuthDomain = EcoTurismo.Domain.Authorization;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class UpdateReservaStatusEndpoint : Endpoint<UpdateReservaStatusRequest>
{
    private readonly IReservaService _service;

    public UpdateReservaStatusEndpoint(IReservaService service) => _service = service;

    public override void Configure()
    {
        Put("/api/reservas/{Id}/status");
        Permissions(AuthDomain.Permissions.ReservasUpdate);
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
