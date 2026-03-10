using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Application.Services;

public class QuiosqueService : IQuiosqueService
{
    private readonly EcoTurismoDbContext _db;
    private static readonly ReservaStatus[] ReservasAtivas =
    [
        ReservaStatus.Confirmada,
        ReservaStatus.EmAndamento,
        ReservaStatus.Validada
    ];

    public QuiosqueService(EcoTurismoDbContext db) => _db = db;

    public async Task<List<QuiosqueDto>> ListarAsync(Guid? atrativoId, DateOnly? dataReferencia = null)
    {
        var query = _db.Quiosques.AsQueryable();

        if (atrativoId.HasValue)
            query = query.Where(q => q.AtrativoId == atrativoId.Value);

        var quiosques = await query
            .OrderBy(q => q.Numero)
            .ToListAsync();

        if (quiosques.Count == 0)
            return [];

        var referencia = dataReferencia ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var idsOperacionais = quiosques
            .Where(q => q.Status is (int)QuiosqueStatus.Disponivel or (int)QuiosqueStatus.Ocupado)
            .Select(q => q.Id)
            .ToList();

        HashSet<Guid> ocupadosNaData = [];

        if (idsOperacionais.Count > 0)
        {
            var ocupados = await _db.Reservas
                .Where(r =>
                    r.QuiosqueId.HasValue &&
                    idsOperacionais.Contains(r.QuiosqueId.Value) &&
                    r.Data <= referencia &&
                    (r.DataFim ?? r.Data) >= referencia &&
                    ReservasAtivas.Contains(r.Status))
                .Select(r => r.QuiosqueId!.Value)
                .Distinct()
                .ToListAsync();

            ocupadosNaData = ocupados.ToHashSet();
        }

        return quiosques
            .Select(q =>
            {
                var statusEfetivo = q.Status;
                if (q.Status is (int)QuiosqueStatus.Disponivel or (int)QuiosqueStatus.Ocupado)
                {
                    statusEfetivo = ocupadosNaData.Contains(q.Id)
                        ? (int)QuiosqueStatus.Ocupado
                        : (int)QuiosqueStatus.Disponivel;
                }

                return MapToDto(q, statusEfetivo);
            })
            .ToList();
    }

    public async Task<QuiosqueDto> CriarAsync(QuiosqueCreateRequest request)
    {
        var quiosque = new Quiosque
        {
            Id = Guid.NewGuid(),
            AtrativoId = request.AtrativoId,
            Numero = request.Numero,
            TemChurrasqueira = request.TemChurrasqueira,
            Status = request.Status,
            PosicaoX = request.PosicaoX,
            PosicaoY = request.PosicaoY,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        _db.Quiosques.Add(quiosque);
        await _db.SaveChangesAsync();

        return MapToDto(quiosque);
    }

    public async Task<QuiosqueDto?> AtualizarAsync(Guid id, QuiosqueUpdateRequest request)
    {
        var q = await _db.Quiosques.FindAsync(id);
        if (q is null) return null;

        if (request.Numero.HasValue) q.Numero = request.Numero.Value;
        if (request.TemChurrasqueira.HasValue) q.TemChurrasqueira = request.TemChurrasqueira.Value;
        if (request.Status.HasValue)
        {
            var statusSolicitado = request.Status.Value;

            // Status "disponivel" e "ocupado" sao derivados de reservas ativas do dia.
            if (statusSolicitado == (int)QuiosqueStatus.Disponivel || statusSolicitado == (int)QuiosqueStatus.Ocupado)
            {
                var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
                var possuiReservaAtivaHoje = await _db.Reservas.AnyAsync(r =>
                    r.QuiosqueId == id &&
                    r.Data <= hoje &&
                    (r.DataFim ?? r.Data) >= hoje &&
                    ReservasAtivas.Contains(r.Status));

                q.Status = possuiReservaAtivaHoje
                    ? (int)QuiosqueStatus.Ocupado
                    : (int)QuiosqueStatus.Disponivel;
            }
            else
            {
                q.Status = statusSolicitado;
            }
        }
        if (request.PosicaoX.HasValue) q.PosicaoX = request.PosicaoX.Value;
        if (request.PosicaoY.HasValue) q.PosicaoY = request.PosicaoY.Value;
        q.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();
        return MapToDto(q);
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var q = await _db.Quiosques.FindAsync(id);
        if (q is null) return false;

        _db.Quiosques.Remove(q);
        await _db.SaveChangesAsync();
        return true;
    }

    private static QuiosqueDto MapToDto(Quiosque q, int? statusOverride = null) => new(
        q.Id, q.AtrativoId, q.Numero,
        q.TemChurrasqueira, ((QuiosqueStatus)(statusOverride ?? q.Status)).ToString(),
        q.PosicaoX, q.PosicaoY
    );
}
