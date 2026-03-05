namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class ReordenarImagensRequest
{
    public Guid AtrativoId { get; set; }
    public List<ImagemOrdemDto> Imagens { get; set; } = new();
}

public record ImagemOrdemDto(string Id, int Ordem);
