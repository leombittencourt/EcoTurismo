namespace EcoTurismo.Application.DTOs;

public record ValidacaoRequest(string Token, Guid? AtrativoId);

public record ValidacaoResponse(bool Valido, string Mensagem, ReservaDto? Reserva);
