using EcoTurismo.Api.Authorization;
using EcoTurismo.Api.Helpers;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Uploads;

public class UploadBannerEndpoint : Endpoint<UploadBannerRequest, BannerDto>
{
    private readonly EcoTurismoDbContext _db;
    private readonly IImageService _imageService;

    public UploadBannerEndpoint(EcoTurismoDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    public override void Configure()
    {
        Post("/api/uploads/banners");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        AllowFileUploads();
        Description(d => d
            .WithTags("Uploads", "Banners")
            .WithSummary("Upload de banner com imagem")
            .Produces<BannerDto>(201)
            .Produces(400)
            .Produces(404));
    }

    public override async Task HandleAsync(UploadBannerRequest req, CancellationToken ct)
    {
        // Validar se o município existe (se informado)
        if (req.MunicipioId.HasValue)
        {
            var municipio = await _db.Municipios
                .FirstOrDefaultAsync(m => m.Id == req.MunicipioId.Value, ct);

            if (municipio is null)
            {
                ThrowError("Município não encontrado.");
                return;
            }
        }

        // Determinar a ordem
        var query = _db.Banners.AsQueryable();
        if (req.MunicipioId.HasValue)
            query = query.Where(b => b.MunicipioId == req.MunicipioId.Value);

        var maxOrdem = await query.MaxAsync(b => (int?)b.Ordem, ct) ?? 0;

        // Criar o banner primeiro (sem imagem)
        var banner = new Banner
        {
            Id = Guid.NewGuid(),
            MunicipioId = req.MunicipioId,
            Titulo = req.Titulo,
            Subtitulo = req.Subtitulo,
            Link = req.Link,
            Ordem = req.Ordem ?? maxOrdem + 1,
            Ativo = req.Ativo ?? true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _db.Banners.Add(banner);
        await _db.SaveChangesAsync(ct);

        // Fazer upload da imagem usando IImageService
        try
        {
            using var memoryStream = new MemoryStream();
            await req.Imagem.CopyToAsync(memoryStream, ct);
            var bytes = memoryStream.ToArray();

            var uploadRequest = new ImagemUploadRequest(
                EntidadeTipo: "Banner",
                EntidadeId: banner.Id,
                Categoria: "principal",
                ImagemBytes: bytes,
                NomeArquivo: req.Imagem.FileName,
                TipoMime: req.Imagem.ContentType,
                Ordem: 0
            );

            var result = await _imageService.SalvarImagemAsync(uploadRequest);

            if (!result.Success)
            {
                // Reverter criação do banner se falhar o upload da imagem
                _db.Banners.Remove(banner);
                await _db.SaveChangesAsync(ct);
                ThrowError($"Erro ao processar a imagem: {result.ErrorMessage}");
                return;
            }

            // Atualizar banner com a referência da imagem
            banner.ImagemId = result.Data!.Id;
            await _db.SaveChangesAsync(ct);

            // Recarregar banner com imagem incluída
            banner = (await _db.Banners
                .Include(b => b.Imagem)
                .FirstOrDefaultAsync(b => b.Id == banner.Id, ct))!;
        }
        catch (Exception ex)
        {
            // Reverter criação do banner em caso de erro
            _db.Banners.Remove(banner);
            await _db.SaveChangesAsync(ct);
            ThrowError($"Erro ao processar a imagem: {ex.Message}");
            return;
        }

        HttpContext.Response.StatusCode = 201;
        HttpContext.Response.Headers.Add("Location", $"/api/banners/{banner.Id}");
        await Send.OkAsync(banner.ToDto(), ct);
    }
}
