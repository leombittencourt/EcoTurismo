using EcoTurismo.Api.Authorization;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

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

        var imagens = await _db.Imagens
            .Where(i => i.EntidadeTipo == "Atrativo" && i.EntidadeId == req.AtrativoId)
            .ToListAsync(ct);

        if (!imagens.Any())
        {
            ThrowError("Atrativo não possui imagens.");
            return;
        }

        var imagemSelecionada = imagens.FirstOrDefault(i => i.Id == req.ImagemId);
        if (imagemSelecionada is null)
        {
            ThrowError("Imagem não encontrada.");
            return;
        }

        // Desmarcar todas as imagens
        foreach (var img in imagens)
        {
            if (img.Categoria == "principal")
            {
                img.Categoria = "galeria";
                img.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        // Marcar apenas a imagem especificada como principal
        imagemSelecionada.Categoria = "principal";
        imagemSelecionada.UpdatedAt = DateTimeOffset.UtcNow;

        atrativo.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
}
