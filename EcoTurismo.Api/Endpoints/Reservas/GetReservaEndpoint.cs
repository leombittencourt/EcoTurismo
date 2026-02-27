using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class GetReservaRequest
{
    public Guid Id { get; set; }
}

public class GetReservaEndpoint : Endpoint<GetReservaRequest, ReservaDto>
{
    private readonly IReservaService _service;

    public GetReservaEndpoint(IReservaService service) => _service = service;

    public override void Configure()
    {
        Get("/api/reservas/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetReservaRequest req, CancellationToken ct)
    {
        var list = await _service.ListarAsync(null);
        var reserva = list.FirstOrDefault(r => r.Id == req.Id);

        if (reserva is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(reserva, ct);
    }
}
