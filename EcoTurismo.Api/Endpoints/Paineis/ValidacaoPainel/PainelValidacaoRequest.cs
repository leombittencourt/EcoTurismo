using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Paineis;

public class PainelValidacaoRequest
{
    [QueryParam]
    public Guid AtrativoId { get; set; }
    
    [QueryParam]
    public DateOnly? Data { get; set; }
    
    [QueryParam]
    public bool IncluirReservas { get; set; } = false;
}
