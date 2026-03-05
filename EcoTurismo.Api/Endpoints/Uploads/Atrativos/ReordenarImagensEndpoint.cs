using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class ReordenarImagensEndpoint : Endpoint<ReordenarImagensRequest>
{
    private readonly EcoTurismoDbContext _db;

    public ReordenarImagensEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Put("/api/uploads/atrativos/{AtrativoId}/imagens/reordenar");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        Description(d => d
            .WithTags("Uploads", "Atrativos")
            .WithSummary("Reordena as imagens do atrativo")
            .WithDescription("Atualiza a ordem de exibição das imagens")
            .Produces(200)
            .Produces(400)
            .Produces(404));
    }

    public override async Task HandleAsync(ReordenarImagensRequest req, CancellationToken ct)
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

        // Verificar se todos os IDs existem
        var idsInvalidos = req.Imagens.Where(r => !imagens.Any(i => i.Id == r.Id)).ToList();
        if (idsInvalidos.Any())
        {
            ThrowError($"IDs de imagens inválidos: {string.Join(", ", idsInvalidos.Select(i => i.Id))}");
            return;
        }

        // Atualizar ordens
        imagens = imagens.Select(img =>
        {
            var novaOrdem = req.Imagens.FirstOrDefault(r => r.Id == img.Id);
            return novaOrdem != null ? img with { Ordem = novaOrdem.Ordem } : img;
        }).OrderBy(i => i.Ordem).ToList();

        atrativo.Imagens = JsonSerializer.Serialize(imagens);
        atrativo.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
}
