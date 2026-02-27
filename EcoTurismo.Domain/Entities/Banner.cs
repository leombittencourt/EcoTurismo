namespace EcoTurismo.Domain.Entities;

public class Banner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public string ImagemUrl { get; set; } = string.Empty;
    public string? Link { get; set; }
    public int Ordem { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
