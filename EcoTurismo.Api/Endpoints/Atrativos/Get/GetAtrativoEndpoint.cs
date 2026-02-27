using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class GetAtrativoEndpoint : Endpoint<GetAtrativoRequest, AtrativoDto>
{
    private readonly EcoTurismoDbContext _db;

    public GetAtrativoEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/atrativos/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAtrativoRequest req, CancellationToken ct)
    {
        var a = await _db.Atrativos.FindAsync([req.Id], ct);

        if (a is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new AtrativoDto(
            a.Id, a.Nome, a.Tipo, a.MunicipioId,
            a.CapacidadeMaxima, a.OcupacaoAtual, a.Status,
            a.Descricao, a.Imagem
        ), ct);
    }
}
