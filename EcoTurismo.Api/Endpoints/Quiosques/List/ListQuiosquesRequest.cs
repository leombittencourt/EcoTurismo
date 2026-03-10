using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class ListQuiosquesRequest
{
    [RouteParam]
    public Guid AtrativoId { get; set; }

    [QueryParam]
    public string? Data { get; set; }
}
