using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class ListQuiosquesEndpoint : Endpoint<ListQuiosquesRequest, List<QuiosqueDto>>
{
    private readonly IQuiosqueService _service;

    public ListQuiosquesEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Get("/api/quiosques");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ListQuiosquesRequest req, CancellationToken ct)
    {
        var data = await _service.ListarAsync(req.AtrativoId);
        await Send.OkAsync(data, ct);
    }
}
