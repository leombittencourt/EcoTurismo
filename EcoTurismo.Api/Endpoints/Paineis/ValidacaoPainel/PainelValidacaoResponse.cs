namespace EcoTurismo.Api.Endpoints.Paineis;

public class PainelValidacaoResponse
{
    public Guid AtrativoId { get; set; }
    public string NomeAtrativo { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public int Validadas { get; set; }
    public int Recusadas { get; set; }
    public int Pendentes { get; set; }
    public int OcupacaoAtual { get; set; }
    public int CapacidadeMaxima { get; set; }
    public decimal PercentualOcupacao { get; set; }
    public int TotalReservasDia { get; set; }
    public List<ReservaDoDiaDto>? ReservasDoDia { get; set; }
}

public class ReservaDoDiaDto
{
    public Guid Id { get; set; }
    public string NomeVisitante { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int QuantidadePessoas { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDescricao { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public Guid? QuiosqueId { get; set; }
    public int? NumeroQuiosque { get; set; }
}
