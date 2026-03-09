using EcoTurismo.Domain.Enums;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class GetReservaByTokenResponse
{
    public Guid Id { get; set; }
    public string NomeVisitante { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public DateOnly? DataFim { get; set; }
    public string Tipo { get; set; } = "day_use";
    public int QuantidadePessoas { get; set; } = 1;
    public string Status { get; set; } = ReservaStatus.Confirmada.ToStringValue();
    public string Token { get; set; } = string.Empty;
    public string AtrativoNome { get; set; } = string.Empty;
    public int? QuiosqueNumero { get; set; }
    public bool QuiosqueChurrasqueira { get; set; }
}
