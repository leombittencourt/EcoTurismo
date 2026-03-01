using Microsoft.AspNetCore.Http;

namespace EcoTurismo.Api.Endpoints.Uploads.Municipios;

public class UploadLogoPublicoRequest
{
    public Guid MunicipioId { get; set; }
    public IFormFile Logo { get; set; } = null!;
}
