using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Enums;

namespace EcoTurismo.Application.Interfaces;

public interface IReservaService
{
    Task<List<ReservaDto>> ListarAsync(Guid? atrativoId);
    Task<ServiceResult<ReservaDto>> CriarAsync(ReservaCreateRequest request);
    Task<ServiceResult<bool>> AtualizarStatusAsync(Guid id, ReservaStatus status);
    Task<ValidacaoResponse> ValidarTicketAsync(ValidacaoRequest request, Guid? operadorId);
}
