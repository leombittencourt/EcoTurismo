using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using FastEndpoints;
using System.Globalization;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class ListQuiosquesEndpoint : Endpoint<ListQuiosquesRequest, List<QuiosqueDto>>
{
    private readonly IQuiosqueService _service;

    public ListQuiosquesEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Get("/api/quiosques-atrativo/{atrativoId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ListQuiosquesRequest req, CancellationToken ct)
    {
        DateOnly? dataReferencia = null;

        var dataParam = req.Data;
        if (!string.IsNullOrWhiteSpace(dataParam))
        {
            if (!DateOnly.TryParseExact(dataParam, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                ThrowError("Formato de data inválido. Use yyyy-MM-dd.");
            }

            dataReferencia = parsed;
        }

        var data = await _service.ListarAsync(req.AtrativoId, dataReferencia);
        await Send.OkAsync(data, ct);
    }
}
