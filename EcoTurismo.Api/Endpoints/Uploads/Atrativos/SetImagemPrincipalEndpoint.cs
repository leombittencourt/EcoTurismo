using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class SetImagemPrincipalEndpoint : Endpoint<SetImagemPrincipalRequest>
{
    private readonly EcoTurismoDbContext _db;

    public SetImagemPrincipalEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Put("/api/uploads/atrativos/{AtrativoId}/imagens/{ImagemId}/principal");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        Description(d => d
            .WithTags("Uploads", "Atrativos")
            .WithSummary("Define uma imagem como principal")
            .WithDescription("Marca apenas a imagem especificada como principal, desmarcando as outras")
            .Produces(200)
            .Produces(404));
    }

    public override async Task HandleAsync(SetImagemPrincipalRequest req, CancellationToken ct)
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

        if (!imagens.Any(i => i.Id == req.ImagemId))
        {
            ThrowError("Imagem não encontrada.");
            return;
        }

        // Marcar apenas a imagem especificada como principal
        imagens = imagens.Select(i => i with { Principal = i.Id == req.ImagemId }).ToList();

        atrativo.Imagens = JsonSerializer.Serialize(imagens);
        atrativo.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
}
