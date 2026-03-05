namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class DeleteImagemAtrativoRequest
{
    public Guid AtrativoId { get; set; }
    public string ImagemId { get; set; } = string.Empty;
}
