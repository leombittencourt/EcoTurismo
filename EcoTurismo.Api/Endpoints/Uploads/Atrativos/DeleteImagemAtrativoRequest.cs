namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class DeleteImagemAtrativoRequest
{
    public Guid AtrativoId { get; set; }
    public Guid ImagemId { get; set; }
}
