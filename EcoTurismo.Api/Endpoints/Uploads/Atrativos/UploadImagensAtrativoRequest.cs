using Microsoft.AspNetCore.Http;

namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class UploadImagensAtrativoRequest
{
    public Guid AtrativoId { get; set; }
    public IFormFile[] Imagens { get; set; } = Array.Empty<IFormFile>();
    public string[]? Descricoes { get; set; }
    public int[]? Ordens { get; set; }
    public string? PrincipalId { get; set; }
}
