using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;

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
        var b = await _db.Banners.FindAsync([req.Id], ct);

        if (b is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new BannerDto(b.Id, b.Titulo, b.Subtitulo, b.ImagemUrl, b.Link, b.Ordem, b.Ativo), ct);
    }
}
