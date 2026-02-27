namespace EcoTurismo.Application.DTOs;

public record AtrativoDto(
    Guid Id,
    string Nome,
    string Tipo,
    Guid MunicipioId,
    int CapacidadeMaxima,
    int OcupacaoAtual,
    string Status,
    string? Descricao,
    string? Imagem
);

public record AtrativoUpdateRequest
{
    public string? Nome { get; init; }
    public string? Tipo { get; init; }
    public string? Descricao { get; init; }
    public string? Imagem { get; init; }
    public int? CapacidadeMaxima { get; init; }
    public int? OcupacaoAtual { get; init; }
    public string? Status { get; init; }
}
