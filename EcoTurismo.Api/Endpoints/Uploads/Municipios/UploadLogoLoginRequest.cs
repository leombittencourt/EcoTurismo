using Microsoft.AspNetCore.Http;

namespace EcoTurismo.Api.Endpoints.Uploads.Municipios;

public class UploadLogoLoginRequest
{
    public Guid MunicipioId { get; set; }
    public IFormFile Logo { get; set; } = null!;
}
