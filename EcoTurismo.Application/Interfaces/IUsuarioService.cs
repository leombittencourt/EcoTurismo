using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Application.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioListItem>> ListarAsync();
    Task<UsuarioDto?> ObterPorIdAsync(Guid id);
    Task<UsuarioDto> CriarAsync(UsuarioCreateRequest request);
    Task<UsuarioDto?> AtualizarAsync(Guid id, UsuarioUpdateRequest request);
    Task<bool> ExcluirAsync(Guid id);
}
