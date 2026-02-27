using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class ListReservasEndpoint : Endpoint<ListReservasRequest, List<ReservaDto>>
{
    private readonly IReservaService _service;

    public ListReservasEndpoint(IReservaService service) => _service = service;

    public override void Configure()
    {
        Get("/api/reservas");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ListReservasRequest req, CancellationToken ct)
    {
        var data = await _service.ListarAsync(req.AtrativoId);
        await Send.OkAsync(data, ct);
    }
}
