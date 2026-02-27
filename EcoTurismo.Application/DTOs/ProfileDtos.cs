namespace EcoTurismo.Application.DTOs;

public record ProfileDto(
    Guid Id,
    string Nome,
    string Email,
    string Role,
    Guid? MunicipioId,
    Guid? AtrativoId
);
