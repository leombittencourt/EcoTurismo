namespace EcoTurismo.Api.Endpoints.Reservas;

public class UpdateReservaStatusRequest
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}
