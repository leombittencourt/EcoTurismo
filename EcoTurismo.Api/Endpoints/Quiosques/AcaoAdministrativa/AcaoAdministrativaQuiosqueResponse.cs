using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class AcaoAdministrativaQuiosqueResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public int ReservasAfetadas { get; set; }
    public QuiosqueDto? Quiosque { get; set; }
}
