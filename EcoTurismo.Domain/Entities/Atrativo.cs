using EcoTurismo.Domain.Enums;

namespace EcoTurismo.Domain.Entities;

public class Atrativo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MunicipioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoAtrativo Tipo { get; set; } = TipoAtrativo.Balneario;
    public string? Descricao { get; set; }
    public string? Endereco { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? MapUrl { get; set; }
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
