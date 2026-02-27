namespace EcoTurismo.Api.Endpoints.Profiles;

public class MeResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid? MunicipioId { get; set; }
    public Guid? AtrativoId { get; set; }
}
