namespace EcoTurismo.Application.DTOs;

/// <summary>
/// DTO completo de Município com imagens separadas
/// </summary>
public record MunicipioDto(
    Guid Id,
    string Nome,
    string Uf,
    ImagemDto? Logo,
    ImagemDto? LogoTelaLogin,
    ImagemDto? LogoAreaPublica
);

/// <summary>
/// DTO simplificado de Município
/// </summary>
public record MunicipioSimplifiedDto(
    Guid Id,
    string Nome,
    string Uf,
    Guid? LogoId,
    Guid? LogoTelaLoginId,
    Guid? LogoAreaPublicaId
);
