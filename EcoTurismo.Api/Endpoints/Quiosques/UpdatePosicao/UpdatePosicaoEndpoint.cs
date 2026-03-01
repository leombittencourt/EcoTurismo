using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

/// <summary>
/// Atualiza a posição de um único quiosque
/// </summary>
public class UpdatePosicaoEndpoint : Endpoint<UpdatePosicaoRequest, QuiosqueDto>
{
    private readonly EcoTurismoDbContext _db;

    public UpdatePosicaoEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Patch("/api/quiosques/{Id}/posicao");
        Policies(RolePolicies.AdminOrBalnearioPolicy);
    }

    public override async Task HandleAsync(UpdatePosicaoRequest req, CancellationToken ct)
    {
        var quiosque = await _db.Quiosques.FindAsync([req.Id], ct);

        if (quiosque is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        quiosque.PosicaoX = req.PosicaoX;
        quiosque.PosicaoY = req.PosicaoY;
        quiosque.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);

        var dto = new QuiosqueDto(
            quiosque.Id,
            quiosque.AtrativoId,
            quiosque.Numero,
            quiosque.TemChurrasqueira,
            ((QuiosqueStatus)quiosque.Status).ToString(),
            quiosque.PosicaoX,
            quiosque.PosicaoY
        );

        await Send.OkAsync(dto, ct);
    }
}
