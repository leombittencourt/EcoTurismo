namespace EcoTurismo.Api.Endpoints.Banners;

public class UpdateBannerRequest
{
    public Guid Id { get; set; }
    public string? Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public string? ImagemUrl { get; set; }
    public string? Link { get; set; }
    public int? Ordem { get; set; }
    public bool? Ativo { get; set; }
}
