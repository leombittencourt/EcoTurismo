namespace EcoTurismo.Application.DTOs;

public record ImagemAtrativoDto(
    string Id,
    string Url,
    int Ordem,
    bool Principal,
    string? Descricao
);
