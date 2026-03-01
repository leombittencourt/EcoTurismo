using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Uploads;

public class UploadBannerEndpoint : Endpoint<UploadBannerRequest, BannerDto>
{
    private readonly EcoTurismoDbContext _db;

    public UploadBannerEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/api/uploads/banners");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        AllowFileUploads();
        Description(d => d
            .WithTags("Uploads", "Banners")
            .WithSummary("Upload de banner (converte para base64 automaticamente)")
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

        // Converter IFormFile para base64
        string imagemBase64;
        try
        {
            using var memoryStream = new MemoryStream();
            await req.Imagem.CopyToAsync(memoryStream, ct);
            var bytes = memoryStream.ToArray();
            var base64String = Convert.ToBase64String(bytes);

            // Criar o formato data URI completo
            var contentType = req.Imagem.ContentType;
            imagemBase64 = $"data:{contentType};base64,{base64String}";
        }
        catch (Exception ex)
        {
            ThrowError($"Erro ao processar a imagem: {ex.Message}");
            return;
        }

        // Determinar a ordem
        var query = _db.Banners.AsQueryable();
        if (req.MunicipioId.HasValue)
            query = query.Where(b => b.MunicipioId == req.MunicipioId.Value);

        var maxOrdem = await query.MaxAsync(b => (int?)b.Ordem, ct) ?? 0;

        // Criar banner com a imagem base64
        var banner = new Banner
        {
            Id = Guid.NewGuid(),
            MunicipioId = req.MunicipioId,
            Titulo = req.Titulo,
            Subtitulo = req.Subtitulo,
            ImagemUrl = imagemBase64, // Salvar o base64 completo com data URI
            Link = req.Link,
            Ordem = req.Ordem ?? maxOrdem + 1,
            Ativo = req.Ativo ?? true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _db.Banners.Add(banner);
        await _db.SaveChangesAsync(ct);

        var dto = new BannerDto(
            banner.Id,
            banner.MunicipioId,
            banner.Titulo,
            banner.Subtitulo,
            banner.ImagemUrl,
            banner.Link,
            banner.Ordem,
            banner.Ativo
        );

        HttpContext.Response.StatusCode = 201;
        HttpContext.Response.Headers.Add("Location", $"/api/banners/{banner.Id}");
        await Send.OkAsync(dto, ct);
    }
}
