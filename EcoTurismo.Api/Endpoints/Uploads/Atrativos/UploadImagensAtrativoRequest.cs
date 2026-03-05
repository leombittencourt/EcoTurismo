using Microsoft.AspNetCore.Http;

namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class UploadImagensAtrativoRequest
{
    public Guid AtrativoId { get; set; }
    public List<IFormFile> Imagens { get; set; } = new();
    public string[]? Descricoes { get; set; }
    public int[]? Ordens { get; set; }
    public string? PrincipalId { get; set; }
}
