using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Configuracoes;

public class ListConfiguracoesEndpoint : EndpointWithoutRequest<List<ConfiguracaoDto>>
{
    private readonly EcoTurismoDbContext _db;

    public ListConfiguracoesEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/configuracoes");
        Policies(RolePolicies.AdminOrPrefeituraPolicy); // Apenas Admin ou Prefeitura podem ver configurações
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var data = await _db.Configuracoes
            .Select(c => new ConfiguracaoDto(c.Chave, c.Valor))
            .ToListAsync(ct);

        await Send.OkAsync(data, ct);
    }
}
