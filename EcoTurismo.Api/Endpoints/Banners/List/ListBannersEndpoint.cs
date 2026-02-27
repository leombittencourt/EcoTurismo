using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Banners;

public class ListBannersEndpoint : Endpoint<ListBannersRequest, List<BannerDto>>
{
    private readonly EcoTurismoDbContext _db;

    public ListBannersEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/banners");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ListBannersRequest req, CancellationToken ct)
    {
        var query = _db.Banners.AsQueryable();

        if (req.ApenasAtivos == true)
            query = query.Where(b => b.Ativo);

        var data = await query
            .OrderBy(b => b.Ordem)
            .Select(b => new BannerDto(b.Id, b.Titulo, b.Subtitulo, b.ImagemUrl, b.Link, b.Ordem, b.Ativo))
            .ToListAsync(ct);

        await Send.OkAsync(data, ct);
    }
}
