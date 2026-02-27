using System.Text.Json.Serialization;

namespace EcoTurismo.Domain.Entities;

public class Municipio
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation (ignorar para evitar ciclos)
    [JsonIgnore]
    public ICollection<Atrativo> Atrativos { get; set; } = new List<Atrativo>();

    [JsonIgnore]
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
