namespace EcoTurismo.Domain.ValueObjects;

/// <summary>
/// Metadados de uma imagem
/// </summary>
public class ImageMetadata
{
    /// <summary>
    /// Nome do arquivo original
    /// </summary>
    public string NomeArquivo { get; set; } = string.Empty;
    
    /// <summary>
    /// Tamanho do arquivo em bytes
    /// </summary>
    public long TamanhoBytes { get; set; }
    
    /// <summary>
    /// Tipo MIME (ex: image/png, image/jpeg)
    /// </summary>
    public string TipoMime { get; set; } = string.Empty;
    
    /// <summary>
    /// Largura da imagem original em pixels
    /// </summary>
    public int LarguraOriginal { get; set; }
    
    /// <summary>
    /// Altura da imagem original em pixels
    /// </summary>
    public int AlturaOriginal { get; set; }
    
    /// <summary>
    /// Largura do thumbnail em pixels (se houver)
    /// </summary>
    public int? LarguraThumbnail { get; set; }
    
    /// <summary>
    /// Altura do thumbnail em pixels (se houver)
    /// </summary>
    public int? AlturaThumbnail { get; set; }
    
    /// <summary>
    /// Data/hora do upload
    /// </summary>
    public DateTimeOffset DataUpload { get; set; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// Hash MD5 da imagem para detecção de duplicatas
    /// </summary>
    public string? HashMd5 { get; set; }

    /// <summary>
    /// Provedor de armazenamento utilizado (base64, oci, s3, azure)
    /// Opcional para compatibilidade com dados legados
    /// </summary>
    public string? StorageProvider { get; set; }

    /// <summary>
    /// Bucket/Container onde a imagem está armazenada (para provedores cloud)
    /// </summary>
    public string? StorageBucket { get; set; }

    /// <summary>
    /// Path/Key da imagem no storage (para provedores cloud)
    /// </summary>
    public string? StoragePath { get; set; }
}
