using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Banners;

public class CreateBannerEndpoint : Endpoint<CreateBannerRequest, BannerDto>
{
    private readonly EcoTurismoDbContext _db;

    public CreateBannerEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Post("/api/banners");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
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
            ImagemUrl = req.ImagemUrl,
            Link = req.Link,
            Ordem = req.Ordem ?? maxOrdem + 1,
            Ativo = req.Ativo ?? true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        _db.Banners.Add(banner);
        await _db.SaveChangesAsync(ct);

        await Send.CreatedAtAsync<GetBannerEndpoint>(
            new { id = banner.Id },
            new BannerDto(banner.Id, banner.MunicipioId, banner.Titulo, banner.Subtitulo, banner.ImagemUrl, banner.Link, banner.Ordem, banner.Ativo),
            cancellation: ct
        );
    }
}
