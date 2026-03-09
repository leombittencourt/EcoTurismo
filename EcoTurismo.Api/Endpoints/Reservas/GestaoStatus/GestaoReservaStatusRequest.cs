namespace EcoTurismo.Api.Endpoints.Reservas;

public class GestaoReservaStatusRequest
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
}
