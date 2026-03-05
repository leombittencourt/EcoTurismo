using EcoTurismo.Domain.Enums;

namespace EcoTurismo.Application.DTOs;

public record AtrativoCreateRequest
{
    public Guid MunicipioId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public TipoAtrativo Tipo { get; init; } = TipoAtrativo.Balneario;
    public string? Descricao { get; init; }
    public string? Endereco { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public string? MapUrl { get; init; }
    public string? Imagem { get; init; }
    public int CapacidadeMaxima { get; init; }
}
