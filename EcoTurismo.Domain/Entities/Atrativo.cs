namespace EcoTurismo.Domain.Entities;

public class Atrativo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MunicipioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = "balneario";
    public string? Descricao { get; set; }
    public string? Imagem { get; set; }
    public string? Imagens { get; set; }
    public int CapacidadeMaxima { get; set; }
    public int OcupacaoAtual { get; set; }
    public string Status { get; set; } = "ativo";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation
    public Municipio Municipio { get; set; } = null!;
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Quiosque> Quiosques { get; set; } = new List<Quiosque>();
}
