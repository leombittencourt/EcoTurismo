using System.Security.Cryptography;
using System.Text.Json;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Domain.ValueObjects;
using EcoTurismo.Infra.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace EcoTurismo.Application.Services;

public class ImageService : IImageService
{
    private readonly EcoTurismoDbContext _db;
    private readonly ILogger<ImageService> _logger;
    private readonly IStorageProvider _storageProvider;

    private static readonly string[] FormatosPermitidos = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long TamanhoMaximoBytes = 5 * 1024 * 1024; // 5MB
    private const int ThumbnailLarguraMaxima = 400;
    private const int ThumbnailAlturaMaxima = 400;

    public ImageService(
        EcoTurismoDbContext db, 
        ILogger<ImageService> logger,
        IStorageProvider storageProvider)
    {
        _db = db;
        _logger = logger;
        _storageProvider = storageProvider;
    }

    public Task<ServiceResult<bool>> ValidarImagemAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Task.FromResult(ServiceResult<bool>.Error("Arquivo não fornecido ou vazio."));
        }

        // Validar extensão
        var extensao = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!FormatosPermitidos.Contains(extensao))
        {
            return Task.FromResult(ServiceResult<bool>.Error(
                $"Formato de arquivo não suportado. Use: {string.Join(", ", FormatosPermitidos)}"));
        }

        // Validar tamanho
        if (file.Length > TamanhoMaximoBytes)
        {
            var tamanhoMB = TamanhoMaximoBytes / (1024.0 * 1024.0);
            return Task.FromResult(ServiceResult<bool>.Error(
                $"Arquivo muito grande. Tamanho máximo: {tamanhoMB:F1}MB"));
        }

        return Task.FromResult(ServiceResult<bool>.Ok(true));
    }

    public async Task<ServiceResult<ImagemProcessadaResult>> ProcessarImagemAsync(
        byte[] imagemBytes,
        string nomeArquivo,
        string tipoMime,
        bool gerarThumbnail = true)
    {
        try
        {
            using var bitmap = SKBitmap.Decode(imagemBytes);

            if (bitmap == null)
            {
                return ServiceResult<ImagemProcessadaResult>.Error("Não foi possível decodificar a imagem");
            }

            var metadados = new ImageMetadata
            {
                NomeArquivo = nomeArquivo,
                TamanhoBytes = imagemBytes.Length,
                TipoMime = tipoMime,
                LarguraOriginal = bitmap.Width,
                AlturaOriginal = bitmap.Height,
                DataUpload = DateTimeOffset.UtcNow,
                HashMd5 = CalcularMd5Hash(imagemBytes),
                StorageProvider = _storageProvider.ProviderName ?? "base64"
            };

            // Salvar imagem original usando o storage provider
            var imagemUrl = await _storageProvider.SaveImageAsync(
                imagemBytes, 
                nomeArquivo, 
                tipoMime
            );

            string? thumbnailUrl = null;

            // Gerar e salvar thumbnail se solicitado
            if (gerarThumbnail)
            {
                var thumbnailBytes = GerarThumbnailBytes(bitmap, tipoMime);
                if (thumbnailBytes.HasValue)
                {
                    var thumbnailFileName = $"thumb_{nomeArquivo}";
                    thumbnailUrl = await _storageProvider.SaveImageAsync(
                        thumbnailBytes.Value.Bytes,
                        thumbnailFileName,
                        tipoMime
                    );

                    metadados.LarguraThumbnail = thumbnailBytes.Value.Largura;
                    metadados.AlturaThumbnail = thumbnailBytes.Value.Altura;
                }
            }

            var resultado = new ImagemProcessadaResult(
                imagemUrl,
                thumbnailUrl,
                metadados
            );

            _logger.LogInformation(
                "Imagem processada com {Provider}: {Nome}, Tamanho: {Tamanho}KB, Dimensões: {Largura}x{Altura}",
                _storageProvider.ProviderName,
                nomeArquivo,
                imagemBytes.Length / 1024,
                bitmap.Width,
                bitmap.Height
            );

            return ServiceResult<ImagemProcessadaResult>.Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar imagem: {Nome}", nomeArquivo);
            return ServiceResult<ImagemProcessadaResult>.Error($"Erro ao processar imagem: {ex.Message}");
        }
    }

    public async Task<ServiceResult<ImagemDto>> SalvarImagemAsync(ImagemUploadRequest request)
    {
        try
        {
            // Processar a imagem
            var processamentoResult = await ProcessarImagemAsync(
                request.ImagemBytes,
                request.NomeArquivo,
                request.TipoMime
            );

            if (!processamentoResult.Success)
            {
                return ServiceResult<ImagemDto>.Error(processamentoResult.ErrorMessage!);
            }

            var processado = processamentoResult.Data!;

            // Criar entidade Imagem
            var imagem = new Imagem
            {
                Id = Guid.NewGuid(),
                EntidadeTipo = request.EntidadeTipo,
                EntidadeId = request.EntidadeId,
                Categoria = request.Categoria,
                ImagemUrl = processado.ImagemUrlOriginal,
                ThumbnailUrl = processado.ThumbnailUrl,
                StorageProvider = _storageProvider.ProviderName,
                Ordem = request.Ordem,
                MetadadosJson = JsonSerializer.Serialize(processado.Metadados),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _db.Imagens.Add(imagem);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Imagem salva: {Id} para {Entidade}:{EntidadeId}, Categoria: {Categoria}",
                imagem.Id,
                request.EntidadeTipo,
                request.EntidadeId,
                request.Categoria
            );

            var dto = MapToDto(imagem, processado.Metadados);
            return ServiceResult<ImagemDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar imagem");
            return ServiceResult<ImagemDto>.Error($"Erro ao salvar imagem: {ex.Message}");
        }
    }

    public async Task<List<ImagemDto>> ListarImagensAsync(string entidadeTipo, Guid entidadeId)
    {
        var imagens = await _db.Imagens
            .Where(i => i.EntidadeTipo == entidadeTipo && i.EntidadeId == entidadeId)
            .OrderBy(i => i.Ordem)
            .ThenBy(i => i.CreatedAt)
            .ToListAsync();

        return imagens.Select(i => MapToDto(i)).ToList();
    }

    public async Task<List<ImagemDto>> ListarImagensPorCategoriaAsync(
        string entidadeTipo,
        Guid entidadeId,
        string categoria)
    {
        var imagens = await _db.Imagens
            .Where(i => i.EntidadeTipo == entidadeTipo 
                     && i.EntidadeId == entidadeId 
                     && i.Categoria == categoria)
            .OrderBy(i => i.Ordem)
            .ThenBy(i => i.CreatedAt)
            .ToListAsync();

        return imagens.Select(i => MapToDto(i)).ToList();
    }

    public async Task<ServiceResult<ImagemDto>> ObterImagemAsync(Guid imagemId)
    {
        var imagem = await _db.Imagens.FindAsync(imagemId);

        if (imagem == null)
        {
            return ServiceResult<ImagemDto>.Error("Imagem não encontrada.");
        }

        return ServiceResult<ImagemDto>.Ok(MapToDto(imagem));
    }

    public async Task<ServiceResult<bool>> RemoverImagemAsync(Guid imagemId)
    {
        var imagem = await _db.Imagens.FindAsync(imagemId);

        if (imagem == null)
        {
            return ServiceResult<bool>.Error("Imagem não encontrada.");
        }

        _db.Imagens.Remove(imagem);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Imagem removida: {Id}", imagemId);

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<int>> RemoverImagensEntidadeAsync(string entidadeTipo, Guid entidadeId)
    {
        var imagens = await _db.Imagens
            .Where(i => i.EntidadeTipo == entidadeTipo && i.EntidadeId == entidadeId)
            .ToListAsync();

        if (imagens.Count == 0)
        {
            return ServiceResult<int>.Ok(0);
        }

        _db.Imagens.RemoveRange(imagens);
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Removidas {Count} imagens da entidade {Entidade}:{Id}",
            imagens.Count,
            entidadeTipo,
            entidadeId
        );

        return ServiceResult<int>.Ok(imagens.Count);
    }

    public async Task<ServiceResult<bool>> AtualizarOrdemImagensAsync(
        List<(Guid ImagemId, int NovaOrdem)> ordens)
    {
        try
        {
            var ids = ordens.Select(o => o.ImagemId).ToList();
            var imagens = await _db.Imagens
                .Where(i => ids.Contains(i.Id))
                .ToListAsync();

            foreach (var (imagemId, novaOrdem) in ordens)
            {
                var imagem = imagens.FirstOrDefault(i => i.Id == imagemId);
                if (imagem != null)
                {
                    imagem.Ordem = novaOrdem;
                    imagem.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("Ordem de {Count} imagens atualizada", ordens.Count);

            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar ordem das imagens");
            return ServiceResult<bool>.Error($"Erro ao atualizar ordem: {ex.Message}");
        }
    }

    // ── Métodos Auxiliares ──

    private (byte[] Bytes, int Largura, int Altura)? GerarThumbnailBytes(
        SKBitmap bitmap,
        string tipoMime)
    {
        try
        {
            // Calcular dimensões mantendo aspect ratio
            var (novaLargura, novaAltura) = CalcularDimensoesThumbnail(
                bitmap.Width,
                bitmap.Height,
                ThumbnailLarguraMaxima,
                ThumbnailAlturaMaxima
            );

            // Criar thumbnail redimensionado
            using var resizedBitmap = bitmap.Resize(new SKImageInfo(novaLargura, novaAltura), SKFilterQuality.High);
            if (resizedBitmap == null)
            {
                return null;
            }

            // Converter para bytes
            using var image = SKImage.FromBitmap(resizedBitmap);
            using var data = image.Encode(GetSkiaFormat(tipoMime), 85);
            var thumbnailBytes = data.ToArray();

            return (thumbnailBytes, novaLargura, novaAltura);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao gerar thumbnail");
            return null;
        }
    }

    private static (int Largura, int Altura) CalcularDimensoesThumbnail(
        int larguraOriginal,
        int alturaOriginal,
        int larguraMaxima,
        int alturaMaxima)
    {
        // Se já é menor que o máximo, manter dimensões
        if (larguraOriginal <= larguraMaxima && alturaOriginal <= alturaMaxima)
        {
            return (larguraOriginal, alturaOriginal);
        }

        var ratioLargura = (double)larguraMaxima / larguraOriginal;
        var ratioAltura = (double)alturaMaxima / alturaOriginal;
        var ratio = Math.Min(ratioLargura, ratioAltura);

        var novaLargura = (int)(larguraOriginal * ratio);
        var novaAltura = (int)(alturaOriginal * ratio);

        return (novaLargura, novaAltura);
    }

    private static SKEncodedImageFormat GetSkiaFormat(string tipoMime)
    {
        return tipoMime.ToLowerInvariant() switch
        {
            "image/png" => SKEncodedImageFormat.Png,
            "image/jpeg" or "image/jpg" => SKEncodedImageFormat.Jpeg,
            "image/gif" => SKEncodedImageFormat.Gif,
            "image/webp" => SKEncodedImageFormat.Webp,
            _ => SKEncodedImageFormat.Png
        };
    }

    private static string CalcularMd5Hash(byte[] bytes)
    {
        var hash = MD5.HashData(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    private static ImagemDto MapToDto(Imagem imagem, ImageMetadata? metadados = null)
    {
        metadados ??= string.IsNullOrWhiteSpace(imagem.MetadadosJson)
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
            metadados,
            imagem.CreatedAt
        );
    }
}
