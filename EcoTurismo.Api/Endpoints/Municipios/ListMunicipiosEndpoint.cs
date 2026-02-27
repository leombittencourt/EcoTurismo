using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Municipios;

public class ListMunicipiosResponse
{
    public List<MunicipioDto> Municipios { get; set; } = [];
}

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
        var data = await _db.Municipios
            .OrderBy(m => m.Nome)
            .Select(m => new MunicipioDto(m.Id, m.Nome, m.Uf, m.Logo))
            .ToListAsync(ct);

        await Send.OkAsync(data, ct);
    }
}
