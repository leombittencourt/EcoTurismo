namespace EcoTurismo.Domain.Entities;

/// <summary>
/// Representa uma imagem armazenada no sistema
/// Usado por múltiplas entidades (Banner, Atrativo, Municipio, etc)
/// </summary>
public class Imagem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Tipo da entidade que possui esta imagem (ex: "Banner", "Atrativo", "Municipio")
    /// </summary>
    public string EntidadeTipo { get; set; } = string.Empty;
    
    /// <summary>
    /// ID da entidade que possui esta imagem
    /// </summary>
    public Guid EntidadeId { get; set; }
    
    /// <summary>
    /// Classificação da imagem (ex: "principal", "thumbnail", "galeria", "logo_login", "logo_publico")
    /// </summary>
    public string Categoria { get; set; } = "principal";
    
    /// <summary>
    /// URI ou caminho da imagem (pode ser base64, URL de OCI, S3, etc)
    /// Formato base64: data:image/png;base64,...
    /// Formato OCI/S3: https://bucket.region.oci.com/path/to/image.jpg
    /// </summary>
    public string ImagemUrl { get; set; } = string.Empty;

    /// <summary>
    /// URI ou caminho da versão redimensionada (thumbnail/preview)
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Provedor de armazenamento (base64, oci, s3, azure, local, etc)
    /// Opcional para compatibilidade com dados legados
    /// </summary>
    public string? StorageProvider { get; set; }
    
    /// <summary>
    /// Ordem de exibição (para galerias)
    /// </summary>
    public int Ordem { get; set; }
    
    /// <summary>
    /// Metadados da imagem em JSON
    /// </summary>
    public string MetadadosJson { get; set; } = "{}";
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
