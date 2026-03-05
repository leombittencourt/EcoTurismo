using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class GetImagensAtrativoEndpoint : EndpointWithoutRequest<List<ImagemAtrativoDto>>
{
    private readonly EcoTurismoDbContext _db;

    public GetImagensAtrativoEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/api/uploads/atrativos/{AtrativoId}/imagens");
        AllowAnonymous();
        Description(d => d
            .WithTags("Uploads", "Atrativos")
            .WithSummary("Lista as imagens de um atrativo")
            .WithDescription("Retorna todas as imagens do atrativo ordenadas")
            .Produces(200)
            .Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var atrativoId = Route<Guid>("AtrativoId");

        var atrativoExiste = await _db.Atrativos
            .AnyAsync(a => a.Id == atrativoId, ct);

        if (!atrativoExiste)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var imagens = await _db.Imagens
            .Where(i => i.EntidadeTipo == "Atrativo" && i.EntidadeId == atrativoId)
            .OrderBy(i => i.Ordem)
            .Select(i => new ImagemAtrativoDto(
                i.Id.ToString(),
                i.ImagemUrl,
                i.Ordem,
                i.Categoria == "principal",
                null // Descrição pode ser extraída de MetadadosJson se necessário
            ))
            .ToListAsync(ct);

        await Send.OkAsync(imagens, ct);
    }
}
