namespace EcoTurismo.Api.Endpoints.Reservas;

public class CreateReservaRequest
{
    public Guid AtrativoId { get; set; }
    public Guid? QuiosqueId { get; set; }
    public string NomeVisitante { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string CidadeOrigem { get; set; } = string.Empty;
    public string UfOrigem { get; set; } = string.Empty;
    public string Tipo { get; set; } = "day_use";
    public DateOnly Data { get; set; }
    public DateOnly? DataFim { get; set; }
    public int QuantidadePessoas { get; set; } = 1;
}
