using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class ListQuiosquesEndpoint : EndpointWithoutRequest<List<QuiosqueDto>>
{
    private readonly IQuiosqueService _service;

    public ListQuiosquesEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Get("/api/quiosques-atrativo/{atrativoId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var atrativoId = Route<Guid>("atrativoId");
        var data = await _service.ListarAsync(atrativoId);
        await Send.OkAsync(data, ct);
    }
}
