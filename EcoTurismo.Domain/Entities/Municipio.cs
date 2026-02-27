namespace EcoTurismo.Domain.Entities;

public class Municipio
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation
    public ICollection<Atrativo> Atrativos { get; set; } = new List<Atrativo>();
    public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
