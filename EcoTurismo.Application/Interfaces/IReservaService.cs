using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Application.Interfaces;

public interface IReservaService
{
    Task<List<ReservaDto>> ListarAsync(Guid? atrativoId);
    Task<ReservaDto> CriarAsync(ReservaCreateRequest request);
    Task<bool> AtualizarStatusAsync(Guid id, string status);
    Task<ValidacaoResponse> ValidarTicketAsync(ValidacaoRequest request, Guid? operadorId);
}
