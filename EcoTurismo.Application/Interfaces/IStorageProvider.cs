namespace EcoTurismo.Application.Interfaces;

/// <summary>
/// Interface para provedores de armazenamento de imagens
/// Permite implementações diferentes: Base64, OCI, S3, Azure Blob, Local, etc
/// </summary>
public interface IStorageProvider
{
    /// <summary>
    /// Nome do provedor (base64, oci, s3, azure, local)
    /// </summary>
    string ProviderName { get; }
    
    /// <summary>
    /// Salva uma imagem e retorna a URL/URI de acesso
    /// </summary>
    /// <param name="imageBytes">Bytes da imagem</param>
    /// <param name="fileName">Nome do arquivo (com extensão)</param>
    /// <param name="contentType">Tipo MIME da imagem</param>
    /// <param name="path">Path/pasta onde salvar (opcional)</param>
    /// <returns>URL ou URI de acesso à imagem</returns>
    Task<string> SaveImageAsync(byte[] imageBytes, string fileName, string contentType, string? path = null);
    
    /// <summary>
    /// Remove uma imagem do armazenamento
    /// </summary>
    /// <param name="imageUrl">URL/URI da imagem a ser removida</param>
    Task<bool> DeleteImageAsync(string imageUrl);
    
    /// <summary>
    /// Obtém uma imagem como bytes (útil para provedores externos)
    /// </summary>
    /// <param name="imageUrl">URL/URI da imagem</param>
    Task<byte[]?> GetImageBytesAsync(string imageUrl);
    
    /// <summary>
    /// Verifica se uma imagem existe
    /// </summary>
    /// <param name="imageUrl">URL/URI da imagem</param>
    Task<bool> ExistsAsync(string imageUrl);
}
