using EcoTurismo.Domain.Enums;

namespace EcoTurismo.Application.DTOs;

public record AtrativoUpdateRequest
{
    public string? Nome { get; init; }
    public TipoAtrativo? Tipo { get; init; }
    public string? Descricao { get; init; }
    public string? Imagem { get; init; }
    public int? CapacidadeMaxima { get; init; }
    public int? OcupacaoAtual { get; init; }
    public string? Status { get; init; }
}
