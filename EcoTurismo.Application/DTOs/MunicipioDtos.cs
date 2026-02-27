namespace EcoTurismo.Application.DTOs;

public record MunicipioDto(
    Guid Id,
    string Nome,
    string Uf,
    string? Logo
);
