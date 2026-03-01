using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class ListAtrativosEndpoint : EndpointWithoutRequest<List<AtrativoDto>>
{
    private readonly EcoTurismoDbContext _db;

    public ListAtrativosEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/atrativos-municipio/{municipioId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = _db.Atrativos.AsQueryable();

        var municipioId = Route<Guid?>("municipioId");

        if (municipioId.HasValue)
            query = query.Where(a => a.MunicipioId == municipioId.Value);

        var dataHoje = DateOnly.FromDateTime(DateTime.Today);

        var data = await query
            .OrderBy(a => a.Nome)
            .Select(a => new AtrativoDto(
                a.Id,
                a.Nome,
                a.Tipo,
                a.MunicipioId,
                a.CapacidadeMaxima,
                // Calcular ocupação atual baseada nas reservas ativas de hoje
                a.Reservas
                    .Where(r => r.Data == dataHoje && 
                               (r.Status == ReservaStatus.Confirmada || 
                                r.Status == ReservaStatus.EmAndamento || 
                                r.Status == ReservaStatus.Validada))
                    .Sum(r => r.QuantidadePessoas),
                a.Status,
                a.Descricao,
                a.Imagem
            ))
            .ToListAsync(ct);

        await Send.OkAsync(data, ct);
    }
}
