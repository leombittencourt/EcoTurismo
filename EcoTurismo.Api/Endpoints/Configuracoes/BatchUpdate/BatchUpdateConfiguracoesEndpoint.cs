using EcoTurismo.Api.Authorization;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Configuracoes;

public class BatchUpdateConfiguracoesEndpoint : Endpoint<BatchUpdateConfiguracoesRequest>
{
    private readonly EcoTurismoDbContext _db;

    public BatchUpdateConfiguracoesEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Put("/api/configuracoes");
        Policies(RolePolicies.AdminPolicy); // Apenas Admin pode alterar configurações
    }

    public override async Task HandleAsync(BatchUpdateConfiguracoesRequest req, CancellationToken ct)
    {
        foreach (var item in req.Configs)
        {
            var config = await _db.Configuracoes
                .FirstOrDefaultAsync(c => c.Chave == item.Chave, ct);

            if (config is not null)
            {
                // Atualizar configuração existente
                config.Valor = item.Valor;
                config.UpdatedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                // Criar nova configuração
                var novaConfig = new ConfiguracaoSistema
                {
                    Id = Guid.NewGuid(),
                    Chave = item.Chave,
                    Valor = item.Valor,
                    Descricao = null,
                    UpdatedAt = DateTimeOffset.UtcNow
                };
                await _db.Configuracoes.AddAsync(novaConfig, ct);
            }
        }

        await _db.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}
