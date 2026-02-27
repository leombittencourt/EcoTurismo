using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class ListAtrativosEndpoint : Endpoint<ListAtrativosRequest, List<AtrativoDto>>
{
    private readonly EcoTurismoDbContext _db;

    public ListAtrativosEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/atrativos");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ListAtrativosRequest req, CancellationToken ct)
    {
        var query = _db.Atrativos.AsQueryable();

        if (req.MunicipioId.HasValue)
            query = query.Where(a => a.MunicipioId == req.MunicipioId.Value);

        var data = await query
            .OrderBy(a => a.Nome)
            .Select(a => new AtrativoDto(
                a.Id, a.Nome, a.Tipo, a.MunicipioId,
                a.CapacidadeMaxima, a.OcupacaoAtual, a.Status,
                a.Descricao, a.Imagem
            ))
            .ToListAsync(ct);

        await Send.OkAsync(data, ct);
    }
}
