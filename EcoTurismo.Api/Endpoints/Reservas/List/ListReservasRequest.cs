using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class ListReservasRequest
{
    [QueryParam]
    public Guid? AtrativoId { get; set; }
}
