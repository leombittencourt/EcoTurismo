using System.Security.Claims;
using System.Text.Json;
using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class GestaoReservaStatusEndpoint : Endpoint<GestaoReservaStatusRequest, GestaoReservaStatusResponse>
{
    private readonly EcoTurismoDbContext _db;
    private readonly IReservaService _reservaService;

    public GestaoReservaStatusEndpoint(EcoTurismoDbContext db, IReservaService reservaService)
    {
        _db = db;
        _reservaService = reservaService;
    }

    public override void Configure()
    {
        Patch("/api/reservas/{Id}/gestao-status");
        Policies(RolePolicies.AdminOrBalnearioPolicy);
    }

    public override async Task HandleAsync(GestaoReservaStatusRequest req, CancellationToken ct)
    {
        var reservaAntes = await _db.Reservas
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == req.Id, ct);

        if (reservaAntes is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var statusNovo = ReservaStatusExtensions.FromString(req.Status);
        var statusAnterior = reservaAntes.Status;

        var result = await _reservaService.AtualizarStatusAsync(req.Id, statusNovo);
        if (!result.Success)
        {
            ThrowError(result.ErrorMessage ?? "Não foi possível atualizar o status da reserva.");
        }

        var reservaDepois = await _db.Reservas
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == req.Id, ct);

        if (reservaDepois is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _db.AuditoriasStatusReservas.Add(new AuditoriaStatusReserva
        {
            Id = Guid.NewGuid(),
            ReservaId = req.Id,
            UsuarioId = Guid.TryParse(usuarioId, out var uid) ? uid : null,
            UsuarioNome = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            UsuarioRole = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role_name") ?? string.Empty,
            StatusAnterior = statusAnterior.ToStringValue(),
            StatusNovo = statusNovo.ToStringValue(),
            Motivo = req.Motivo.Trim(),
            Payload = JsonSerializer.Serialize(new
            {
                antes = new
                {
                    reservaAntes.Id,
                    Status = reservaAntes.Status.ToStringValue(),
                    reservaAntes.AtrativoId,
                    reservaAntes.QuiosqueId,
                    reservaAntes.Data,
                    reservaAntes.DataFim,
                    reservaAntes.QuantidadePessoas
                },
                depois = new
                {
                    reservaDepois.Id,
                    Status = reservaDepois.Status.ToStringValue(),
                    reservaDepois.AtrativoId,
                    reservaDepois.QuiosqueId,
                    reservaDepois.Data,
                    reservaDepois.DataFim,
                    reservaDepois.QuantidadePessoas
                }
            }),
            CreatedAt = DateTimeOffset.UtcNow
        });

        await _db.SaveChangesAsync(ct);

        await Send.OkAsync(new GestaoReservaStatusResponse
        {
            Success = true,
            Message = "Status da reserva atualizado com sucesso.",
            StatusAnterior = statusAnterior.ToStringValue(),
            StatusNovo = statusNovo.ToStringValue()
        }, ct);
    }
}
