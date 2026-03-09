namespace EcoTurismo.Domain.Entities;

public class AuditoriaAcaoQuiosque
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? QuiosqueId { get; set; }
    public Guid? UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string UsuarioRole { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public int ReservasAfetadas { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation
    public Quiosque? Quiosque { get; set; }
    public Usuario? Usuario { get; set; }
}
