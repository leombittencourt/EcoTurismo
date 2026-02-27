using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Api.Endpoints.Validacoes;

public class ValidarTicketResponse
{
    public bool Valido { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public ReservaDto? Reserva { get; set; }
}
