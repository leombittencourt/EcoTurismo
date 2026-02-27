using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
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
            Status = "confirmada",
            Token = Guid.NewGuid().ToString("N")[..12].ToUpper(),
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _db.Reservas.Add(reserva);
        await _db.SaveChangesAsync();

        return MapToDto(reserva);
    }

    public async Task<bool> AtualizarStatusAsync(Guid id, string status)
    {
        var reserva = await _db.Reservas.FindAsync(id);
        if (reserva is null) return false;

        reserva.Status = status;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ValidacaoResponse> ValidarTicketAsync(ValidacaoRequest request, Guid? operadorId)
    {
        var reserva = await _db.Reservas
            .FirstOrDefaultAsync(r => r.Token == request.Token);

        var valido = reserva is not null
                     && reserva.Status == "confirmada"
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
            reserva!.Status = "utilizada";

        await _db.SaveChangesAsync();

        var mensagem = valido
            ? "Ticket válido. Entrada autorizada."
            : reserva is null
                ? "Ticket não encontrado."
                : reserva.Status != "confirmada"
                    ? $"Ticket já foi {reserva.Status}."
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
        r.Status, r.Token, r.CreatedAt
    );
}
