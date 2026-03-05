using EcoTurismo.Api.Authorization;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

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
            .WithDescription("Remove a imagem da tabela Imagens e reorganiza se necessário")
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

        var imagemRemover = await _db.Imagens
            .FirstOrDefaultAsync(i => 
                i.Id == req.ImagemId && 
                i.EntidadeTipo == "Atrativo" && 
                i.EntidadeId == req.AtrativoId, ct);

        if (imagemRemover is null)
        {
            ThrowError("Imagem não encontrada.");
            return;
        }

        var eraPrincipal = imagemRemover.Categoria == "principal";

        _db.Imagens.Remove(imagemRemover);

        // Se removeu a principal, marcar primeira como principal
        if (eraPrincipal)
        {
            var primeiraImagem = await _db.Imagens
                .Where(i => i.EntidadeTipo == "Atrativo" && i.EntidadeId == req.AtrativoId)
                .OrderBy(i => i.Ordem)
                .FirstOrDefaultAsync(ct);

            if (primeiraImagem is not null)
            {
                primeiraImagem.Categoria = "principal";
                primeiraImagem.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        atrativo.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
    }
}
