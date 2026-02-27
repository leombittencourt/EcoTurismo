using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class ListAtrativosRequest
{
    [QueryParam]
    public Guid? MunicipioId { get; set; }
}
