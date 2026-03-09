using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class GetReservaByTokenEndpoint : Endpoint<GetReservaByTokenRequest, GetReservaByTokenResponse>
{
    private readonly EcoTurismoDbContext _db;

    public GetReservaByTokenEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/api/reservas/ticket/{Token}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetReservaByTokenRequest req, CancellationToken ct)
    {
        var token = req.Token?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(token))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var reserva = await _db.Reservas
            .AsNoTracking()
            .Include(r => r.Atrativo)
            .Include(r => r.Quiosque)
            .FirstOrDefaultAsync(r => r.Token == token, ct);

        if (reserva is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new GetReservaByTokenResponse
        {
            Id = reserva.Id,
            NomeVisitante = reserva.NomeVisitante,
            Email = reserva.Email,
            Data = reserva.Data,
            DataFim = reserva.DataFim,
            Tipo = reserva.Tipo,
            QuantidadePessoas = reserva.QuantidadePessoas,
            Status = reserva.Status.ToStringValue(),
            Token = reserva.Token,
            AtrativoNome = reserva.Atrativo?.Nome ?? "Atrativo",
            QuiosqueNumero = reserva.Quiosque?.Numero,
            QuiosqueChurrasqueira = reserva.Quiosque?.TemChurrasqueira ?? false
        }, ct);
    }
}
