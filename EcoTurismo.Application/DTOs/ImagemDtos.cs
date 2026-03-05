using EcoTurismo.Domain.ValueObjects;

namespace EcoTurismo.Application.DTOs;

/// <summary>
/// DTO para upload de imagem
/// </summary>
public record ImagemUploadRequest(
    string EntidadeTipo,
    Guid EntidadeId,
    string Categoria,
    byte[] ImagemBytes,
    string NomeArquivo,
    string TipoMime,
    int Ordem = 0
);

/// <summary>
/// DTO de resposta após upload
/// </summary>
public record ImagemDto(
    Guid Id,
    string EntidadeTipo,
    Guid EntidadeId,
    string Categoria,
    string ImagemUrl,
    string? ThumbnailUrl,
    string? StorageProvider,
    int Ordem,
    ImageMetadata Metadados,
    DateTimeOffset CreatedAt
);

/// <summary>
/// Resultado do processamento de imagem
/// </summary>
public record ImagemProcessadaResult(
    string ImagemUrlOriginal,
    string? ThumbnailUrl,
    ImageMetadata Metadados
);
