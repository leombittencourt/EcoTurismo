using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class UpdateAtrativoEndpoint : Endpoint<UpdateAtrativoRequest, AtrativoDto>
{
    private readonly EcoTurismoDbContext _db;

    public UpdateAtrativoEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Put("/api/atrativos/{Id}");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
    }

    public override async Task HandleAsync(UpdateAtrativoRequest req, CancellationToken ct)
    {
        var a = await _db.Atrativos.FindAsync([req.Id], ct);

        if (a is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (req.Nome is not null) a.Nome = req.Nome;
        if (req.Tipo is not null) a.Tipo = req.Tipo;
        if (req.Descricao is not null) a.Descricao = req.Descricao;
        if (req.Imagem is not null) a.Imagem = req.Imagem;
        if (req.CapacidadeMaxima.HasValue) a.CapacidadeMaxima = req.CapacidadeMaxima.Value;
        if (req.OcupacaoAtual.HasValue) a.OcupacaoAtual = req.OcupacaoAtual.Value;
        if (req.Status is not null) a.Status = req.Status;

        await _db.SaveChangesAsync(ct);

        await Send.OkAsync(new AtrativoDto(
            a.Id, a.Nome, a.Tipo, a.MunicipioId,
            a.CapacidadeMaxima, a.OcupacaoAtual, a.Status,
            a.Descricao, a.Imagem
        ), ct);
    }
}
