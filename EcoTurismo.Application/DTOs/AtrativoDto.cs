using EcoTurismo.Domain.Enums;

namespace EcoTurismo.Application.DTOs;

public record AtrativoDto(
    Guid Id,
    string Nome,
    TipoAtrativo Tipo,
    Guid MunicipioId,
    int CapacidadeMaxima,
    int OcupacaoAtual,
    string Status,
    string? Descricao,
    string? Endereco,
    decimal? Latitude,
    decimal? Longitude,
    string? MapUrl,
    string? Imagem,
    List<ImagemAtrativoDto>? Imagens
);
