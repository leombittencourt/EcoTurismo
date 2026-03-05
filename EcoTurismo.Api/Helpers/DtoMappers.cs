using System.Text.Json;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Domain.ValueObjects;

namespace EcoTurismo.Api.Helpers;

/// <summary>
/// Helper para conversão de entidades em DTOs
/// </summary>
public static class DtoMappers
{
    /// <summary>
    /// Converte uma entidade Imagem em ImagemDto
    /// </summary>
    public static ImagemDto? ToDto(this Imagem? imagem)
    {
        if (imagem == null) return null;

        var metadata = string.IsNullOrWhiteSpace(imagem.MetadadosJson)
            ? new ImageMetadata()
            : JsonSerializer.Deserialize<ImageMetadata>(imagem.MetadadosJson) ?? new ImageMetadata();

        return new ImagemDto(
            imagem.Id,
            imagem.EntidadeTipo,
            imagem.EntidadeId,
            imagem.Categoria,
            imagem.ImagemUrl,
            imagem.ThumbnailUrl,
            imagem.StorageProvider,
            imagem.Ordem,
            metadata,
            imagem.CreatedAt
        );
    }

    /// <summary>
    /// Converte uma entidade Banner em BannerDto
    /// </summary>
    public static BannerDto ToDto(this Banner banner)
    {
        return new BannerDto(
            banner.Id,
            banner.MunicipioId,
            banner.Titulo,
            banner.Subtitulo,
            banner.Imagem.ToDto(),
            banner.Link,
            banner.Ordem,
            banner.Ativo
        );
    }

    /// <summary>
    /// Converte uma entidade Municipio em MunicipioDto
    /// </summary>
    public static MunicipioDto ToDto(this Municipio municipio)
    {
        return new MunicipioDto(
            municipio.Id,
            municipio.Nome,
            municipio.Uf,
            municipio.Logo.ToDto(),
            municipio.LogoTelaLogin.ToDto(),
            municipio.LogoAreaPublica.ToDto()
        );
    }
}
