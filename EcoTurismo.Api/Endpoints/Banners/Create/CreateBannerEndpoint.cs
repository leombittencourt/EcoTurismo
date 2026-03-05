using EcoTurismo.Api.Authorization;
using EcoTurismo.Api.Helpers;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Banners;

/// <summary>
/// ENDPOINT LEGADO - Criar banner sem upload de imagem
/// Use UploadBannerEndpoint para criar banners com imagem
/// Este endpoint está mantido apenas para compatibilidade
/// </summary>
public class CreateBannerEndpoint : Endpoint<CreateBannerRequest, BannerDto>
{
    private readonly EcoTurismoDbContext _db;

    public CreateBannerEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Post("/api/banners");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        Description(d => d
            .WithTags("Banners")
            .WithSummary("[LEGADO] Criar banner sem imagem - Use POST /api/uploads/banners"));
    }

    public override async Task HandleAsync(CreateBannerRequest req, CancellationToken ct)
    {
        // Validar município se informado
        if (req.MunicipioId.HasValue)
        {
            var municipioExiste = await _db.Municipios
                .AnyAsync(m => m.Id == req.MunicipioId.Value, ct);

            if (!municipioExiste)
            {
                ThrowError("Município não encontrado.");
                return;
            }
        }

        // Calcular ordem baseado no município (se informado) ou geral
        var query = _db.Banners.AsQueryable();
        if (req.MunicipioId.HasValue)
            query = query.Where(b => b.MunicipioId == req.MunicipioId.Value);

        var maxOrdem = await query.MaxAsync(b => (int?)b.Ordem, ct) ?? 0;

        var banner = new Banner
        {
            Id = Guid.NewGuid(),
            MunicipioId = req.MunicipioId,
            Titulo = req.Titulo,
            Subtitulo = req.Subtitulo,
            ImagemId = null, // Sem imagem
            Link = req.Link,
            Ordem = req.Ordem ?? maxOrdem + 1,
            Ativo = req.Ativo ?? true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        _db.Banners.Add(banner);
        await _db.SaveChangesAsync(ct);

        // Recarregar com includes
        banner = (await _db.Banners
            .Include(b => b.Imagem)
            .FirstOrDefaultAsync(b => b.Id == banner.Id, ct))!;

        await Send.CreatedAtAsync<GetBannerEndpoint>(
            new { id = banner.Id },
            banner.ToDto(),
            cancellation: ct
        );
    }
}
