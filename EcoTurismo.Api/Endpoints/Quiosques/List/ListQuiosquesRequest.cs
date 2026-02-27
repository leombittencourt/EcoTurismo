using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class ListQuiosquesRequest
{
    [QueryParam]
    public Guid? AtrativoId { get; set; }
}
