using System.Text.Json.Serialization;

namespace EcoTurismo.Domain.Entities;

public class Municipio
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;

    // FKs para imagens
    public Guid? LogoId { get; set; }
    public Guid? LogoTelaLoginId { get; set; }
    public Guid? LogoAreaPublicaId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation (ignorar para evitar ciclos)
    [JsonIgnore]
    public Imagem? Logo { get; set; }

    [JsonIgnore]
    public Imagem? LogoTelaLogin { get; set; }

    [JsonIgnore]
    public Imagem? LogoAreaPublica { get; set; }

    [JsonIgnore]
    public ICollection<Atrativo> Atrativos { get; set; } = new List<Atrativo>();

    [JsonIgnore]
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    [JsonIgnore]
    public ICollection<Banner> Banners { get; set; } = new List<Banner>();
}
