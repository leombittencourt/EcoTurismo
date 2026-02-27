namespace EcoTurismo.Domain.Entities;

public class Validacao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? ReservaId { get; set; }
    public Guid? AtrativoId { get; set; }
    public Guid? OperadorId { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool Valido { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation
    public Reserva? Reserva { get; set; }
    public Atrativo? Atrativo { get; set; }
    public Profile? Operador { get; set; }
}
