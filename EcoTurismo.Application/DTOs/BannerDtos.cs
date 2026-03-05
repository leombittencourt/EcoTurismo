namespace EcoTurismo.Application.DTOs;

/// <summary>
/// DTO de resposta de Banner com imagem separada
/// </summary>
public record BannerDto(
    Guid Id,
    Guid? MunicipioId,
    string? Titulo,
    string? Subtitulo,
    ImagemDto? Imagem,
    string? Link,
    int Ordem,
    bool Ativo
);

/// <summary>
/// DTO simplificado quando não precisa carregar a imagem completa
/// </summary>
public record BannerSimplifiedDto(
    Guid Id,
    Guid? MunicipioId,
    Guid? ImagemId,
    string? Titulo,
    string? Subtitulo,
    string? Link,
    int Ordem,
    bool Ativo
);

public record BannerCreateRequest
{
    public Guid? MunicipioId { get; init; }
    public string? Titulo { get; init; }
    public string? Subtitulo { get; init; }
    public string? Link { get; init; }
    public int? Ordem { get; init; }
    public bool? Ativo { get; init; }
}

public record BannerUpdateRequest
{
    public string? Titulo { get; init; }
    public string? Subtitulo { get; init; }
    public string? Link { get; init; }
    public int? Ordem { get; init; }
    public bool? Ativo { get; init; }
}

public record BannerReorderItem(Guid Id, int Ordem);

public record BannerReorderRequest(List<BannerReorderItem> Itens);
