namespace EcoTurismo.Domain.Entities;

public class ConfiguracaoSistema
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Chave { get; set; } = string.Empty;
    public string? Valor { get; set; }
    public string? Descricao { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
