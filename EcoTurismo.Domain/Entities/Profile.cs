namespace EcoTurismo.Domain.Entities;

public class Profile
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "publico";
    public Guid? MunicipioId { get; set; }
    public Guid? AtrativoId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string PasswordHash { get; set; } = string.Empty;

    // Navigation
    public Municipio? Municipio { get; set; }
    public Atrativo? Atrativo { get; set; }
}
