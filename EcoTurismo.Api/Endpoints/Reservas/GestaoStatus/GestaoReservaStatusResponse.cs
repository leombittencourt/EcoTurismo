namespace EcoTurismo.Api.Endpoints.Reservas;

public class GestaoReservaStatusResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string StatusAnterior { get; set; } = string.Empty;
    public string StatusNovo { get; set; } = string.Empty;
}
