using System.Data;
using System.Security.Claims;
using System.Text.Json;
using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class AcaoAdministrativaQuiosqueEndpoint : Endpoint<AcaoAdministrativaQuiosqueRequest, AcaoAdministrativaQuiosqueResponse>
{
    private static readonly ReservaStatus[] ReservasAtivas =
    [
        ReservaStatus.Confirmada,
        ReservaStatus.EmAndamento,
        ReservaStatus.Validada
    ];

    private readonly EcoTurismoDbContext _db;
    private readonly IOcupacaoService _ocupacaoService;

    public AcaoAdministrativaQuiosqueEndpoint(EcoTurismoDbContext db, IOcupacaoService ocupacaoService)
    {
        _db = db;
        _ocupacaoService = ocupacaoService;
    }

    public override void Configure()
    {
        Patch("/api/quiosques/{Id}/acao-administrativa");
        Policies(RolePolicies.AdminOrBalnearioPolicy);
    }

    public override async Task HandleAsync(AcaoAdministrativaQuiosqueRequest req, CancellationToken ct)
    {
        var acao = req.Acao.Trim().ToLowerInvariant();
        var strategy = _db.Database.CreateExecutionStrategy();

        var resultado = await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            var quiosque = await _db.Quiosques
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM ""Quiosques""
                    WHERE ""Id"" = {req.Id}
                    FOR UPDATE")
                .SingleOrDefaultAsync(ct);

            if (quiosque is null)
                return (response: (AcaoAdministrativaQuiosqueResponse?)null, notFound: true, conflict: false);

            var agora = DateTimeOffset.UtcNow;
            var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
            var reservasAfetadas = 0;

            var snapshotAntes = new
            {
                quiosque.Id,
                quiosque.AtrativoId,
                quiosque.Numero,
                quiosque.TemChurrasqueira,
                quiosque.Status,
                quiosque.PosicaoX,
                quiosque.PosicaoY
            };

            var reservasAtivasEFuturas = await _db.Reservas
                .Where(r => r.QuiosqueId == quiosque.Id)
                .Where(r => ReservasAtivas.Contains(r.Status))
                .Where(r => (r.DataFim ?? r.Data) >= hoje)
                .ToListAsync(ct);

            if (acao == "inativar")
            {
                quiosque.Status = (int)QuiosqueStatus.Inativo;
            }
            else if (acao == "editar")
            {
                if (req.Numero.HasValue) quiosque.Numero = req.Numero.Value;
                if (req.TemChurrasqueira.HasValue) quiosque.TemChurrasqueira = req.TemChurrasqueira.Value;
                if (req.PosicaoX.HasValue) quiosque.PosicaoX = req.PosicaoX.Value;
                if (req.PosicaoY.HasValue) quiosque.PosicaoY = req.PosicaoY.Value;

                if (req.Status.HasValue)
                {
                    var statusSolicitado = req.Status.Value;

                    if (statusSolicitado == (int)QuiosqueStatus.Disponivel || statusSolicitado == (int)QuiosqueStatus.Ocupado)
                    {
                        var possuiReservaAtivaHoje = await _db.Reservas.AnyAsync(r =>
                            r.QuiosqueId == quiosque.Id &&
                            r.Data <= hoje &&
                            (r.DataFim ?? r.Data) >= hoje &&
                            ReservasAtivas.Contains(r.Status), ct);

                        quiosque.Status = possuiReservaAtivaHoje
                            ? (int)QuiosqueStatus.Ocupado
                            : (int)QuiosqueStatus.Disponivel;
                    }
                    else
                    {
                        quiosque.Status = statusSolicitado;
                    }
                }
            }
            else if (acao == "desvincular_reservas")
            {
                foreach (var reserva in reservasAtivasEFuturas)
                    reserva.QuiosqueId = null;

                reservasAfetadas = reservasAtivasEFuturas.Count;

                quiosque.Status = (int)QuiosqueStatus.Disponivel;
            }
            else if (acao == "excluir")
            {
                if (reservasAtivasEFuturas.Count > 0 && !req.DesvincularReservasAtivasEFuturas)
                    return (response: new AcaoAdministrativaQuiosqueResponse
                    {
                        Success = false,
                        Acao = acao,
                        Message = "Quiosque possui reservas ativas/futuras. Para exclusao forcada, envie DesvincularReservasAtivasEFuturas=true.",
                        ReservasAfetadas = 0
                    }, notFound: false, conflict: true);

                if (req.DesvincularReservasAtivasEFuturas)
                {
                    foreach (var reserva in reservasAtivasEFuturas)
                        reserva.QuiosqueId = null;

                    reservasAfetadas = reservasAtivasEFuturas.Count;
                }

                _db.Quiosques.Remove(quiosque);
            }

            quiosque.UpdatedAt = agora;

            var snapshotDepois = new
            {
                quiosque.Id,
                quiosque.AtrativoId,
                quiosque.Numero,
                quiosque.TemChurrasqueira,
                quiosque.Status,
                quiosque.PosicaoX,
                quiosque.PosicaoY,
                Excluido = acao == "excluir"
            };

            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _db.AuditoriasAcoesQuiosques.Add(new AuditoriaAcaoQuiosque
            {
                Id = Guid.NewGuid(),
                QuiosqueId = quiosque.Id,
                UsuarioId = Guid.TryParse(usuarioId, out var uid) ? uid : null,
                UsuarioNome = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                UsuarioRole = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role_name") ?? string.Empty,
                Acao = acao,
                Motivo = req.Motivo.Trim(),
                ReservasAfetadas = reservasAfetadas,
                Payload = JsonSerializer.Serialize(new { antes = snapshotAntes, depois = snapshotDepois }),
                CreatedAt = agora
            });

            await _db.SaveChangesAsync(ct);

            if (quiosque.AtrativoId.HasValue)
                await _ocupacaoService.ReconciliarOcupacaoAtrativoAsync(quiosque.AtrativoId.Value, ct);

            await tx.CommitAsync(ct);

            var dto = acao == "excluir"
                ? null
                : new QuiosqueDto(
                    quiosque.Id,
                    quiosque.AtrativoId,
                    quiosque.Numero,
                    quiosque.TemChurrasqueira,
                    ((QuiosqueStatus)quiosque.Status).ToStringValue(),
                    quiosque.PosicaoX,
                    quiosque.PosicaoY
                );

            return (response: new AcaoAdministrativaQuiosqueResponse
            {
                Success = true,
                Acao = acao,
                Message = $"Acao administrativa '{acao}' executada com sucesso.",
                ReservasAfetadas = reservasAfetadas,
                Quiosque = dto
            }, notFound: false, conflict: false);
        });

        if (resultado.notFound)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (resultado.conflict)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            await HttpContext.Response.WriteAsJsonAsync(resultado.response!, ct);
            return;
        }

        await Send.OkAsync(resultado.response!, ct);
    }
}
