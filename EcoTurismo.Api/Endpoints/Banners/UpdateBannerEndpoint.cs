using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Banners;

public class UpdateBannerRequest
{
    public Guid Id { get; set; }
    public string? Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public string? ImagemUrl { get; set; }
    public string? Link { get; set; }
    public int? Ordem { get; set; }
    public bool? Ativo { get; set; }
}

public class UpdateBannerValidator : Validator<UpdateBannerRequest>
{
    public UpdateBannerValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");
    }
}

public class UpdateBannerEndpoint : Endpoint<UpdateBannerRequest, BannerDto>
{
    private readonly EcoTurismoDbContext _db;

    public UpdateBannerEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Put("/api/banners/{Id}");
        Roles("admin", "prefeitura");
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

        await Send.OkAsync(new BannerDto(b.Id, b.Titulo, b.Subtitulo, b.ImagemUrl, b.Link, b.Ordem, b.Ativo), ct);
    }
}
