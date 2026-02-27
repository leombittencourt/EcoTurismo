namespace EcoTurismo.Api.Endpoints.Atrativos;

public class UpdateAtrativoRequest
{
    public Guid Id { get; set; }
    public string? Nome { get; set; }
    public string? Tipo { get; set; }
    public string? Descricao { get; set; }
    public string? Imagem { get; set; }
    public int? CapacidadeMaxima { get; set; }
    public int? OcupacaoAtual { get; set; }
    public string? Status { get; set; }
}
