using Microsoft.AspNetCore.Http;

namespace EcoTurismo.Api.Endpoints.Uploads;

public class UploadBannerRequest
{
    public Guid? MunicipioId { get; set; }
    public IFormFile Imagem { get; set; } = null!;
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public string? Link { get; set; }
    public int? Ordem { get; set; }
    public bool? Ativo { get; set; }
}
