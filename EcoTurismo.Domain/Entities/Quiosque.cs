using EcoTurismo.Domain.Enums;

namespace EcoTurismo.Domain.Entities;

public class Quiosque
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? AtrativoId { get; set; }
    public int Numero { get; set; }
    public bool TemChurrasqueira { get; set; }
    public int Status { get; set; } = (int)QuiosqueStatus.Disponivel;
    public int PosicaoX { get; set; }
    public int PosicaoY { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation
    public Atrativo? Atrativo { get; set; }
}
