using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using AuthDomain = EcoTurismo.Domain.Authorization;

namespace EcoTurismo.Api.Endpoints.Banners;

public class CreateBannerEndpoint : Endpoint<CreateBannerRequest, BannerDto>
{
    private readonly EcoTurismoDbContext _db;

    public CreateBannerEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Post("/api/banners");
        Permissions(AuthDomain.Permissions.BannersCreate);
    }

    public override async Task HandleAsync(CreateBannerRequest req, CancellationToken ct)
    {
        var maxOrdem = await _db.Banners.MaxAsync(b => (int?)b.Ordem, ct) ?? 0;

        var banner = new Banner
        {
            Id = Guid.NewGuid(),
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
            new BannerDto(banner.Id, banner.Titulo, banner.Subtitulo, banner.ImagemUrl, banner.Link, banner.Ordem, banner.Ativo),
            cancellation: ct
        );
    }
}
