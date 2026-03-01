namespace EcoTurismo.Application.DTOs;

public record BannerDto(
    Guid Id,
    Guid? MunicipioId,
    string? Titulo,
    string? Subtitulo,
    string ImagemUrl,
    string? Link,
    int Ordem,
    bool Ativo
);

public record BannerCreateRequest
{
    public Guid? MunicipioId { get; init; }
    public string? Titulo { get; init; }
    public string? Subtitulo { get; init; }
    public string ImagemUrl { get; init; } = string.Empty;
    public string? Link { get; init; }
    public int? Ordem { get; init; }
    public bool? Ativo { get; init; }
}

public record BannerUpdateRequest
{
    public string? Titulo { get; init; }
    public string? Subtitulo { get; init; }
    public string? ImagemUrl { get; init; }
    public string? Link { get; init; }
    public int? Ordem { get; init; }
    public bool? Ativo { get; init; }
}

public record BannerReorderItem(Guid Id, int Ordem);

public record BannerReorderRequest(List<BannerReorderItem> Itens);
