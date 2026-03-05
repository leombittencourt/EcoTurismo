using EcoTurismo.Api.Helpers;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Banners;

public class GetBannerEndpoint : Endpoint<GetBannerRequest, BannerDto>
{
    private readonly EcoTurismoDbContext _db;

    public GetBannerEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/banners/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetBannerRequest req, CancellationToken ct)
    {
        var banner = await _db.Banners
            .Include(b => b.Imagem)
            .FirstOrDefaultAsync(b => b.Id == req.Id, ct);

        if (banner is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(banner.ToDto(), ct);
    }
}
