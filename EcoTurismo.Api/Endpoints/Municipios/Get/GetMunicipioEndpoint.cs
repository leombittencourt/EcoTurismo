using EcoTurismo.Api.Helpers;
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
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var municipio = await _db.Municipios
            .Include(m => m.Logo)
            .Include(m => m.LogoTelaLogin)
            .Include(m => m.LogoAreaPublica)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        if (municipio is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(municipio.ToDto(), ct);
    }
}
