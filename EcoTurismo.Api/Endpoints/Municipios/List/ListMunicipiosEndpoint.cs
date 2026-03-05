using EcoTurismo.Api.Helpers;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Municipios;

public class ListMunicipiosEndpoint : EndpointWithoutRequest<List<MunicipioDto>>
{
    private readonly EcoTurismoDbContext _db;

    public ListMunicipiosEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/municipios");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var municipios = await _db.Municipios
            .Include(m => m.Logo)
            .Include(m => m.LogoTelaLogin)
            .Include(m => m.LogoAreaPublica)
            .OrderBy(m => m.Nome)
            .ToListAsync(ct);

        var data = municipios.Select(m => m.ToDto()).ToList();

        await Send.OkAsync(data, ct);
    }
}
