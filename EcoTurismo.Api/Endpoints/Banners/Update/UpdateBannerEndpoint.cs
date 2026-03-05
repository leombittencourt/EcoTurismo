using EcoTurismo.Api.Authorization;
using EcoTurismo.Api.Helpers;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Banners;

public class UpdateBannerEndpoint : Endpoint<UpdateBannerRequest, BannerDto>
{
    private readonly EcoTurismoDbContext _db;

    public UpdateBannerEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Put("/api/banners/{Id}");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
    }

    public override async Task HandleAsync(UpdateBannerRequest req, CancellationToken ct)
    {
        var banner = await _db.Banners
            .Include(b => b.Imagem)
            .FirstOrDefaultAsync(b => b.Id == req.Id, ct);

        if (banner is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (req.Titulo is not null) banner.Titulo = req.Titulo;
        if (req.Subtitulo is not null) banner.Subtitulo = req.Subtitulo;
        if (req.Link is not null) banner.Link = req.Link;
        if (req.Ordem.HasValue) banner.Ordem = req.Ordem.Value;
        if (req.Ativo.HasValue) banner.Ativo = req.Ativo.Value;

        banner.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        await Send.OkAsync(banner.ToDto(), ct);
    }
}
