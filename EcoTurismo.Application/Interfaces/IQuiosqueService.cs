using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Application.Interfaces;

public interface IQuiosqueService
{
    Task<List<QuiosqueDto>> ListarAsync(Guid? atrativoId, DateOnly? dataReferencia = null);
    Task<QuiosqueDto> CriarAsync(QuiosqueCreateRequest request);
    Task<QuiosqueDto?> AtualizarAsync(Guid id, QuiosqueUpdateRequest request);
    Task<bool> ExcluirAsync(Guid id);
}
