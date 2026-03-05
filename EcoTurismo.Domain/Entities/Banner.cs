using System.Text.Json.Serialization;

namespace EcoTurismo.Domain.Entities;

public class Banner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? MunicipioId { get; set; }
    public Guid? ImagemId { get; set; }
    public string? Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public string? Link { get; set; }
    public int Ordem { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation
    [JsonIgnore]
    public Municipio? Municipio { get; set; }

    [JsonIgnore]
    public Imagem? Imagem { get; set; }
}
