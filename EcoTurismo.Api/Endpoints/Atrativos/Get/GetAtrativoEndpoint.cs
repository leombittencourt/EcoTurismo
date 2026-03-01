using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class GetAtrativoEndpoint : EndpointWithoutRequest<AtrativoDto>
{
    private readonly EcoTurismoDbContext _db;

    public GetAtrativoEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/atrativos/{Id}");        
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");

        var dataHoje = DateOnly.FromDateTime(DateTime.Today);

        var atrativo = await _db.Atrativos
            .Where(a => a.Id == id)
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
            .FirstOrDefaultAsync(ct);

        if (atrativo is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(atrativo, ct);
    }
}
