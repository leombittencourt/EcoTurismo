using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class DeleteImagemAtrativoEndpoint : Endpoint<DeleteImagemAtrativoRequest>
{
    private readonly EcoTurismoDbContext _db;

    public DeleteImagemAtrativoEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Delete("/api/uploads/atrativos/{AtrativoId}/imagens/{ImagemId}");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        Description(d => d
            .WithTags("Uploads", "Atrativos")
            .WithSummary("Remove uma imagem específica do atrativo")
            .WithDescription("Remove a imagem do JSON array e reorganiza se necessário")
            .Produces(200)
            .Produces(404));
    }

    public override async Task HandleAsync(DeleteImagemAtrativoRequest req, CancellationToken ct)
    {
        var atrativo = await _db.Atrativos
            .FirstOrDefaultAsync(a => a.Id == req.AtrativoId, ct);

        if (atrativo is null)
        {
            ThrowError("Atrativo não encontrado.");
            return;
        }

        if (string.IsNullOrWhiteSpace(atrativo.Imagens))
        {
            ThrowError("Atrativo não possui imagens.");
            return;
        }

        var imagens = JsonSerializer.Deserialize<List<ImagemAtrativoDto>>(atrativo.Imagens) ?? new List<ImagemAtrativoDto>();

        var imagemRemover = imagens.FirstOrDefault(i => i.Id == req.ImagemId);
        if (imagemRemover is null)
        {
            ThrowError("Imagem não encontrada.");
            return;
        }

        imagens.Remove(imagemRemover);

        // Se removeu a principal e ainda há imagens, marcar primeira como principal
        if (imagemRemover.Principal && imagens.Count > 0)
        {
            var primeira = imagens.OrderBy(i => i.Ordem).First();
            imagens = imagens.Select(i => i.Id == primeira.Id ? i with { Principal = true } : i).ToList();
        }

        atrativo.Imagens = imagens.Count > 0 ? JsonSerializer.Serialize(imagens) : "[]";
        atrativo.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
}
