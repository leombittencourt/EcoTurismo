using System.Data;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Application.Services;

public class ReservaService : IReservaService
{
    private readonly EcoTurismoDbContext _db;

    public ReservaService(EcoTurismoDbContext db) => _db = db;

    public async Task<List<ReservaDto>> ListarAsync(Guid? atrativoId)
    {
        var query = _db.Reservas.AsQueryable();

        if (atrativoId.HasValue)
            query = query.Where(r => r.AtrativoId == atrativoId.Value);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    public async Task<ReservaDto> CriarAsync(ReservaCreateRequest request)
    {
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            Quiosque? quiosque = null;

            if (request.QuiosqueId.HasValue)
            {
                // ✅ Sem SqlRaw: entidade tracked
                // Se você quiser lock pessimista, veja a seção "LOCK" abaixo.
                quiosque = await _db.Quiosques
                    .SingleOrDefaultAsync(q => q.Id == request.QuiosqueId.Value);

                if (quiosque is null)
                    throw new InvalidOperationException("Quiosque não encontrado.");

                var ocupadoNaData = await _db.Reservas.AnyAsync(r =>
                    r.QuiosqueId == request.QuiosqueId.Value &&
                    r.Data == request.Data &&
                    (r.Status == ReservaStatus.Confirmada ||
                     r.Status == ReservaStatus.EmAndamento ||
                     r.Status == ReservaStatus.Validada));

                if (ocupadoNaData)
                    throw new InvalidOperationException($"Quiosque já está ocupado para a data {request.Data:dd/MM/yyyy}");
            }

            var reserva = new Reserva
            {
                Id = Guid.NewGuid(),
                AtrativoId = request.AtrativoId,
                QuiosqueId = request.QuiosqueId,
                NomeVisitante = request.NomeVisitante,
                Email = request.Email,
                Cpf = request.Cpf,
                CidadeOrigem = request.CidadeOrigem,
                UfOrigem = request.UfOrigem,
                Tipo = request.Tipo,
                Data = request.Data,
                DataFim = request.DataFim,
                QuantidadePessoas = request.QuantidadePessoas,
                Status = ReservaStatus.Confirmada,
                Token = Guid.NewGuid().ToString("N")[..12].ToUpper(),
                CreatedAt = DateTimeOffset.UtcNow,
            };

            _db.Reservas.Add(reserva);

            if (quiosque is not null)
                quiosque.Status = (int)QuiosqueStatus.Ocupado;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return MapToDto(reserva);
        });
    }

    public async Task<bool> AtualizarStatusAsync(Guid id, ReservaStatus status)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var reserva = await _db.Reservas.FirstOrDefaultAsync(r => r.Id == id);
        if (reserva is null) return false;

        var statusAnterior = reserva.Status;

        Quiosque? quiosque = null;
        if (reserva.QuiosqueId.HasValue)
        {
            // Lock no quiosque para evitar concorrência entre liberações/ocupações
            quiosque = await _db.Quiosques
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM ""Quiosques""
                    WHERE ""Id"" = {reserva.QuiosqueId.Value}
                    FOR UPDATE")
                .SingleOrDefaultAsync();
        }

        reserva.Status = status;
        await _db.SaveChangesAsync();

        if (quiosque is not null)
        {
            // Se mudou para cancelada ou concluída, liberar se não existir outra ativa na mesma data
            if (status == ReservaStatus.Cancelada || status == ReservaStatus.Concluida)
            {
                var outrasReservasAtivas = await _db.Reservas
                    .AnyAsync(r => r.QuiosqueId == reserva.QuiosqueId!.Value &&
                                   r.Id != reserva.Id &&
                                   r.Data == reserva.Data &&
                                   (r.Status == ReservaStatus.Confirmada ||
                                    r.Status == ReservaStatus.EmAndamento ||
                                    r.Status == ReservaStatus.Validada));

                if (!outrasReservasAtivas)
                    quiosque.Status = (int)QuiosqueStatus.Disponivel;
            }
            // Se saiu de inativo => ativo, ocupar
            else if (!statusAnterior.EstaAtiva() && status.EstaAtiva())
            {
                quiosque.Status = (int)QuiosqueStatus.Ocupado;
            }

            await _db.SaveChangesAsync();
        }

        await tx.CommitAsync();
        return true;
    }

    public async Task<ValidacaoResponse> ValidarTicketAsync(ValidacaoRequest request, Guid? operadorId)
    {
        var reserva = await _db.Reservas
            .FirstOrDefaultAsync(r => r.Token == request.Token);

        var valido = reserva is not null
                     && reserva.Status == ReservaStatus.Confirmada
                     && reserva.Data == DateOnly.FromDateTime(DateTime.UtcNow);

        var validacao = new Validacao
        {
            Id = Guid.NewGuid(),
            ReservaId = reserva?.Id,
            AtrativoId = request.AtrativoId,
            OperadorId = operadorId,
            Token = request.Token,
            Valido = valido,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _db.Validacoes.Add(validacao);

        if (valido)
            reserva!.Status = ReservaStatus.Validada;

        await _db.SaveChangesAsync();

        var mensagem = valido
            ? "Ticket válido. Entrada autorizada."
            : reserva is null
                ? "Ticket não encontrado."
                : reserva.Status != ReservaStatus.Confirmada
                    ? $"Ticket já foi {reserva.Status.ToDescricao()}."
                    : "Ticket não é válido para a data de hoje.";

        return new ValidacaoResponse(
            valido,
            mensagem,
            reserva is not null ? MapToDto(reserva) : null
        );
    }

    private static ReservaDto MapToDto(Reserva r) => new(
        r.Id, r.AtrativoId, r.QuiosqueId, r.NomeVisitante,
        r.Email, r.Cpf, r.CidadeOrigem, r.UfOrigem,
        r.Tipo, r.Data, r.DataFim, r.QuantidadePessoas,
        r.Status.ToStringValue(), r.Token, r.CreatedAt
    );
}