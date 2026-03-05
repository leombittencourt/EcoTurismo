using EcoTurismo.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace EcoTurismo.Application.Interfaces;

public interface IImageService
{
    /// <summary>
    /// Valida se o arquivo é uma imagem válida
    /// </summary>
    Task<ServiceResult<bool>> ValidarImagemAsync(IFormFile file);
    
    /// <summary>
    /// Processa uma imagem: valida, converte para base64, cria thumbnail e extrai metadados
    /// </summary>
    Task<ServiceResult<ImagemProcessadaResult>> ProcessarImagemAsync(
        byte[] imagemBytes, 
        string nomeArquivo, 
        string tipoMime,
        bool gerarThumbnail = true);
    
    /// <summary>
    /// Salva uma imagem no banco de dados
    /// </summary>
    Task<ServiceResult<ImagemDto>> SalvarImagemAsync(ImagemUploadRequest request);
    
    /// <summary>
    /// Lista imagens de uma entidade
    /// </summary>
    Task<List<ImagemDto>> ListarImagensAsync(string entidadeTipo, Guid entidadeId);
    
    /// <summary>
    /// Lista imagens de uma entidade por categoria
    /// </summary>
    Task<List<ImagemDto>> ListarImagensPorCategoriaAsync(
        string entidadeTipo, 
        Guid entidadeId, 
        string categoria);
    
    /// <summary>
    /// Busca uma imagem específica
    /// </summary>
    Task<ServiceResult<ImagemDto>> ObterImagemAsync(Guid imagemId);
    
    /// <summary>
    /// Remove uma imagem
    /// </summary>
    Task<ServiceResult<bool>> RemoverImagemAsync(Guid imagemId);
    
    /// <summary>
    /// Remove todas as imagens de uma entidade
    /// </summary>
    Task<ServiceResult<int>> RemoverImagensEntidadeAsync(string entidadeTipo, Guid entidadeId);
    
    /// <summary>
    /// Atualiza a ordem das imagens
    /// </summary>
    Task<ServiceResult<bool>> AtualizarOrdemImagensAsync(List<(Guid ImagemId, int NovaOrdem)> ordens);
}
