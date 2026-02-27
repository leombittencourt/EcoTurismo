using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Application.Interfaces;

public interface IQuiosqueService
{
    Task<List<QuiosqueDto>> ListarAsync(Guid? atrativoId);
    Task<QuiosqueDto> CriarAsync(QuiosqueCreateRequest request);
    Task<QuiosqueDto?> AtualizarAsync(Guid id, QuiosqueUpdateRequest request);
    Task<bool> ExcluirAsync(Guid id);
}
