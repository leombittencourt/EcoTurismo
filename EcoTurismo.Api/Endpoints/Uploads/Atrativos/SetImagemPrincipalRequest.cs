namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class SetImagemPrincipalRequest
{
    public Guid AtrativoId { get; set; }
    public string ImagemId { get; set; } = string.Empty;
}
