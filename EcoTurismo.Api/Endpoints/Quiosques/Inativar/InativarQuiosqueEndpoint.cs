using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class InativarQuiosqueEndpoint : Endpoint<InativarQuiosqueRequest, QuiosqueDto>
{
    private readonly EcoTurismoDbContext _db;

    public InativarQuiosqueEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Patch("/api/quiosques/{Id}/inativar");
        Policies(RolePolicies.AdminOrBalnearioPolicy);
    }

    public override async Task HandleAsync(InativarQuiosqueRequest req, CancellationToken ct)
    {
        var quiosque = await _db.Quiosques.FindAsync([req.Id], ct);
        if (quiosque is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        quiosque.Status = (int)QuiosqueStatus.Inativo;
        quiosque.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);

        var dto = new QuiosqueDto(
            quiosque.Id,
            quiosque.AtrativoId,
            quiosque.Numero,
            quiosque.TemChurrasqueira,
            QuiosqueStatus.Inativo.ToStringValue(),
            quiosque.PosicaoX,
            quiosque.PosicaoY
        );

        await Send.OkAsync(dto, ct);
    }
}
