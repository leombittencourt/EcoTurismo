using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;

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
        var b = await _db.Banners.FindAsync([req.Id], ct);

        if (b is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (req.Titulo is not null) b.Titulo = req.Titulo;
        if (req.Subtitulo is not null) b.Subtitulo = req.Subtitulo;
        if (req.ImagemUrl is not null) b.ImagemUrl = req.ImagemUrl;
        if (req.Link is not null) b.Link = req.Link;
        if (req.Ordem.HasValue) b.Ordem = req.Ordem.Value;
        if (req.Ativo.HasValue) b.Ativo = req.Ativo.Value;

        await _db.SaveChangesAsync(ct);

        await Send.OkAsync(new BannerDto(b.Id, b.MunicipioId, b.Titulo, b.Subtitulo, b.ImagemUrl, b.Link, b.Ordem, b.Ativo), ct);
    }
}
