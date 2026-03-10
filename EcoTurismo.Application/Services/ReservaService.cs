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
    private readonly IOcupacaoService _ocupacaoService;

    public ReservaService(EcoTurismoDbContext db, IOcupacaoService ocupacaoService)
    {
        _db = db;
        _ocupacaoService = ocupacaoService;
    }

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

    public async Task<ServiceResult<ReservaDto>> CriarAsync(ReservaCreateRequest request)
    {
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            Quiosque? quiosque = null;

            if (request.QuiosqueId.HasValue)
            {
                // âœ… Sem SqlRaw: entidade tracked
                // Se vocÃª quiser lock pessimista, veja a seÃ§Ã£o "LOCK" abaixo.
                quiosque = await _db.Quiosques
                    .SingleOrDefaultAsync(q => q.Id == request.QuiosqueId.Value);

                if (quiosque is null)
                    return ServiceResult<ReservaDto>.Error("Quiosque nÃ£o encontrado.");

                if (quiosque.Status is (int)QuiosqueStatus.Inativo or (int)QuiosqueStatus.Manutencao or (int)QuiosqueStatus.Bloqueado)
                    return ServiceResult<ReservaDto>.Error("Quiosque indisponivel para reserva no momento.");

                var inicioSolicitado = request.Data;
                var fimSolicitado = request.DataFim ?? request.Data;

                var ocupadoNoPeriodo = await _db.Reservas.AnyAsync(r =>
                    r.QuiosqueId == request.QuiosqueId.Value &&
                    r.Data <= fimSolicitado &&
                    (r.DataFim ?? r.Data) >= inicioSolicitado &&
                    (r.Status == ReservaStatus.Confirmada ||
                     r.Status == ReservaStatus.EmAndamento ||
                     r.Status == ReservaStatus.Validada));

                if (ocupadoNoPeriodo)
                    return ServiceResult<ReservaDto>.Error(
                        $"Quiosque ja esta ocupado no periodo {inicioSolicitado:dd/MM/yyyy} - {fimSolicitado:dd/MM/yyyy}");
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

            // Incrementar ocupaÃ§Ã£o do atrativo (somente se for para hoje ou futuro)
            await _ocupacaoService.IncrementarOcupacaoAsync(
                reserva.AtrativoId, 
                reserva.Data, 
                reserva.QuantidadePessoas);

            await tx.CommitAsync();

            return ServiceResult<ReservaDto>.Ok(MapToDto(reserva));
        });
    }
    public async Task<ServiceResult<bool>> AtualizarStatusAsync(Guid id, ReservaStatus status)
    {
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            var reserva = await _db.Reservas.FirstOrDefaultAsync(r => r.Id == id);
            if (reserva is null)
                return ServiceResult<bool>.Error("Reserva não encontrada.");

            var statusAnterior = reserva.Status;

            Quiosque? quiosque = null;
            if (reserva.QuiosqueId.HasValue)
            {
                // Lock no quiosque para evitar concorrencia entre liberacoes/ocupacoes
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

            // Gerenciar ocupacao do atrativo
            var statusesAtivos = new[]
            {
                ReservaStatus.Confirmada,
                ReservaStatus.Validada,
                ReservaStatus.EmAndamento
            };

            var estavivoAntes = statusesAtivos.Contains(statusAnterior);
            var estaAtivoAgora = statusesAtivos.Contains(status);

            // Reativacao precisa respeitar capacidade do atrativo no periodo da reserva.
            if (!estavivoAntes && estaAtivoAgora)
            {
                var atrativo = await _db.Atrativos
                    .FirstOrDefaultAsync(a => a.Id == reserva.AtrativoId);

                if (atrativo is null)
                    return ServiceResult<bool>.Error("Atrativo da reserva não encontrado.");

                var inicioReserva = reserva.Data;
                var fimReserva = reserva.DataFim ?? reserva.Data;

                var reservasAtivasNoPeriodo = await _db.Reservas
                    .Where(r =>
                        r.Id != reserva.Id &&
                        r.AtrativoId == reserva.AtrativoId &&
                        r.Data <= fimReserva &&
                        (r.DataFim ?? r.Data) >= inicioReserva &&
                        (r.Status == ReservaStatus.Confirmada ||
                         r.Status == ReservaStatus.EmAndamento ||
                         r.Status == ReservaStatus.Validada))
                    .Select(r => new
                    {
                        r.Data,
                        DataFim = r.DataFim ?? r.Data,
                        r.QuantidadePessoas
                    })
                    .ToListAsync();

                for (var dia = inicioReserva; dia <= fimReserva; dia = dia.AddDays(1))
                {
                    var ocupacaoNoDia = reservasAtivasNoPeriodo
                        .Where(r => r.Data <= dia && r.DataFim >= dia)
                        .Sum(r => r.QuantidadePessoas);

                    var ocupacaoComReativacao = ocupacaoNoDia + reserva.QuantidadePessoas;
                    if (ocupacaoComReativacao > atrativo.CapacidadeMaxima)
                    {
                        return ServiceResult<bool>.Error(
                            $"Reativação excede a capacidade do atrativo em {dia:dd/MM/yyyy}. Capacidade: {atrativo.CapacidadeMaxima}, ocupação projetada: {ocupacaoComReativacao}.");
                    }
                }
            }

            // Se saiu de ativo para inativo, decrementar
            if (estavivoAntes && !estaAtivoAgora)
            {
                await _ocupacaoService.DecrementarOcupacaoAsync(
                    reserva.AtrativoId,
                    reserva.Data,
                    reserva.QuantidadePessoas);
            }
            // Se entrou de inativo para ativo, incrementar
            else if (!estavivoAntes && estaAtivoAgora)
            {
                await _ocupacaoService.IncrementarOcupacaoAsync(
                    reserva.AtrativoId,
                    reserva.Data,
                    reserva.QuantidadePessoas);
            }

            if (quiosque is not null)
            {
                // Se mudou para cancelada ou concluida, liberar se nao existir outra ativa no mesmo periodo
                if (status == ReservaStatus.Cancelada || status == ReservaStatus.Concluida)
                {
                    var inicioAtual = reserva.Data;
                    var fimAtual = reserva.DataFim ?? reserva.Data;

                    var outrasReservasAtivas = await _db.Reservas
                        .AnyAsync(r => r.QuiosqueId == reserva.QuiosqueId!.Value &&
                                       r.Id != reserva.Id &&
                                       r.Data <= fimAtual &&
                                       (r.DataFim ?? r.Data) >= inicioAtual &&
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
            return ServiceResult<bool>.Ok(true);
        });
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
            ? "Ticket vÃ¡lido. Entrada autorizada."
            : reserva is null
                ? "Ticket nÃ£o encontrado."
                : reserva.Status != ReservaStatus.Confirmada
                    ? $"Ticket jÃ¡ foi {reserva.Status.ToDescricao()}."
                    : "Ticket nÃ£o Ã© vÃ¡lido para a data de hoje.";

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

