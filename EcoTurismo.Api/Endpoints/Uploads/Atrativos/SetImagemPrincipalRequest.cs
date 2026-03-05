namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class SetImagemPrincipalRequest
{
    public Guid AtrativoId { get; set; }
    public Guid ImagemId { get; set; }
}
