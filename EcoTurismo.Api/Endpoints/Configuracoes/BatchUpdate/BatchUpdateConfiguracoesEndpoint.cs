using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using AuthDomain = EcoTurismo.Domain.Authorization;

namespace EcoTurismo.Api.Endpoints.Configuracoes;

public class BatchUpdateConfiguracoesEndpoint : Endpoint<BatchUpdateConfiguracoesRequest>
{
    private readonly EcoTurismoDbContext _db;

    public BatchUpdateConfiguracoesEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Put("/api/configuracoes");
        Permissions(AuthDomain.Permissions.ConfiguracoesUpdate);
    }

    public override async Task HandleAsync(BatchUpdateConfiguracoesRequest req, CancellationToken ct)
    {
        foreach (var item in req.Configs)
        {
            var config = await _db.Configuracoes
                .FirstOrDefaultAsync(c => c.Chave == item.Chave, ct);

            if (config is not null)
                config.Valor = item.Valor;
        }

        await _db.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}
