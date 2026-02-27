namespace EcoTurismo.Api.Endpoints.Banners;

public class CreateBannerRequest
{
    public string? Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public string ImagemUrl { get; set; } = string.Empty;
    public string? Link { get; set; }
    public int? Ordem { get; set; }
    public bool? Ativo { get; set; }
}
