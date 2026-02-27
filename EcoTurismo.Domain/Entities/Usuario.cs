using System.Text.Json.Serialization;

namespace EcoTurismo.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    public Guid RoleId { get; set; }
    public Guid? MunicipioId { get; set; }
    public Guid? AtrativoId { get; set; }
    public string? Telefone { get; set; }
    public string? Cpf { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation (ignorar para evitar ciclos)
    [JsonIgnore]
    public Role Role { get; set; } = null!;

    [JsonIgnore]
    public Municipio? Municipio { get; set; }

    [JsonIgnore]
    public Atrativo? Atrativo { get; set; }
}
