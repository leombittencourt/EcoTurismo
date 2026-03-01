using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Municipios;

public class GetMunicipioRequest
{
    public Guid Id { get; set; }
}

public class GetMunicipioEndpoint : EndpointWithoutRequest<MunicipioDto>
{
    private readonly EcoTurismoDbContext _db;

    public GetMunicipioEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/municipios/{Id}");     
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var municipio = await _db.Municipios
            .Where(m => m.Id == id)
            .Select(m => new MunicipioDto(m.Id, m.Nome, m.Uf, m.Logo))
            .FirstOrDefaultAsync(ct);

        if (municipio is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(municipio, ct);
    }
}
