namespace EcoTurismo.Api.Endpoints.Quiosques;

public class AcaoAdministrativaQuiosqueRequest
{
    public Guid Id { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;

    // Campos opcionais para acao "editar"
    public int? Numero { get; set; }
    public bool? TemChurrasqueira { get; set; }
    public int? Status { get; set; }
    public int? PosicaoX { get; set; }
    public int? PosicaoY { get; set; }

    // Campo opcional para acao "excluir"
    public bool DesvincularReservasAtivasEFuturas { get; set; }
}
