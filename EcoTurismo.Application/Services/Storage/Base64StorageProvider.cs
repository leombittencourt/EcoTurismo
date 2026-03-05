using EcoTurismo.Application.Interfaces;

namespace EcoTurismo.Application.Services.Storage;

/// <summary>
/// Provedor de armazenamento que salva imagens como Base64 inline (data URI)
/// Ideal para prototipagem e imagens pequenas
/// </summary>
public class Base64StorageProvider : IStorageProvider
{
    public string ProviderName => "base64";

    public Task<string> SaveImageAsync(byte[] imageBytes, string fileName, string contentType, string? path = null)
    {
        // Converte para data URI (base64)
        var base64String = Convert.ToBase64String(imageBytes);
        var dataUri = $"data:{contentType};base64,{base64String}";
        
        return Task.FromResult(dataUri);
    }

    public Task<bool> DeleteImageAsync(string imageUrl)
    {
        // Base64 não precisa de remoção física
        return Task.FromResult(true);
    }

    public Task<byte[]?> GetImageBytesAsync(string imageUrl)
    {
        try
        {
            // Extrair base64 do data URI
            if (imageUrl.StartsWith("data:"))
            {
                var base64Data = imageUrl.Split(',')[1];
                var bytes = Convert.FromBase64String(base64Data);
                return Task.FromResult<byte[]?>(bytes);
            }
            
            return Task.FromResult<byte[]?>(null);
        }
        catch
        {
            return Task.FromResult<byte[]?>(null);
        }
    }

    public Task<bool> ExistsAsync(string imageUrl)
    {
        // Se tem o prefixo data:, considera que existe
        return Task.FromResult(imageUrl.StartsWith("data:"));
    }
}
