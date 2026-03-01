using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcoTurismo.Application.Services;

public class OcupacaoService : IOcupacaoService
{
    private readonly EcoTurismoDbContext _db;
    private readonly ILogger<OcupacaoService> _logger;

    private static readonly ReservaStatus[] StatusAtivos =
    [
        ReservaStatus.Confirmada,
        ReservaStatus.Validada,
        ReservaStatus.EmAndamento
    ];

    public OcupacaoService(EcoTurismoDbContext db, ILogger<OcupacaoService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> IncrementarOcupacaoAsync(Guid atrativoId, DateOnly data, int quantidade, CancellationToken ct = default)
    {
        // Só incrementa se for para hoje ou futuro
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        if (data < hoje)
        {
            _logger.LogDebug("Não incrementa ocupação para data passada: {Data}", data);
            return false;
        }

        var atrativo = await _db.Atrativos.FindAsync([atrativoId], ct);
        if (atrativo is null)
        {
            _logger.LogWarning("Atrativo não encontrado: {AtrativoId}", atrativoId);
            return false;
        }

        // Proteger contra ultrapassar capacidade
        var novaOcupacao = atrativo.OcupacaoAtual + quantidade;
        if (novaOcupacao > atrativo.CapacidadeMaxima)
        {
            _logger.LogWarning(
                "Tentativa de ultrapassar capacidade máxima. Atrativo: {AtrativoId}, Atual: {Atual}, Tentando: {Quantidade}, Máximo: {Maximo}",
                atrativoId, atrativo.OcupacaoAtual, quantidade, atrativo.CapacidadeMaxima);
            
            // Incrementa até o máximo permitido
            atrativo.OcupacaoAtual = atrativo.CapacidadeMaxima;
        }
        else
        {
            atrativo.OcupacaoAtual = novaOcupacao;
        }

        atrativo.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Ocupação incrementada. Atrativo: {AtrativoId}, Data: {Data}, Quantidade: {Quantidade}, Nova Ocupação: {Ocupacao}",
            atrativoId, data, quantidade, atrativo.OcupacaoAtual);

        return true;
    }

    public async Task<bool> DecrementarOcupacaoAsync(Guid atrativoId, DateOnly data, int quantidade, CancellationToken ct = default)
    {
        var atrativo = await _db.Atrativos.FindAsync([atrativoId], ct);
        if (atrativo is null)
        {
            _logger.LogWarning("Atrativo não encontrado: {AtrativoId}", atrativoId);
            return false;
        }

        // Proteger contra negativo
        var novaOcupacao = Math.Max(0, atrativo.OcupacaoAtual - quantidade);
        atrativo.OcupacaoAtual = novaOcupacao;
        atrativo.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Ocupação decrementada. Atrativo: {AtrativoId}, Data: {Data}, Quantidade: {Quantidade}, Nova Ocupação: {Ocupacao}",
            atrativoId, data, quantidade, atrativo.OcupacaoAtual);

        return true;
    }

    public async Task ReconciliarOcupacoesAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Iniciando reconciliação de ocupações");

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var atrativos = await _db.Atrativos
            .Where(a => a.Status == "ativo")
            .ToListAsync(ct);

        foreach (var atrativo in atrativos)
        {
            await ReconciliarOcupacaoAtrativoAsync(atrativo.Id, ct);
        }

        _logger.LogInformation("Reconciliação concluída. {Total} atrativos processados", atrativos.Count);
    }

    public async Task ReconciliarOcupacaoAtrativoAsync(Guid atrativoId, CancellationToken ct = default)
    {
        var atrativo = await _db.Atrativos.FindAsync([atrativoId], ct);
        if (atrativo is null) return;

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        // Contar reservas ativas para hoje
        var ocupacaoReal = await _db.Reservas
            .Where(r => r.AtrativoId == atrativoId)
            .Where(r => r.Data == hoje)
            .Where(r => StatusAtivos.Contains(r.Status))
            .SumAsync(r => r.QuantidadePessoas, ct);

        var ocupacaoAnterior = atrativo.OcupacaoAtual;

        if (ocupacaoReal != ocupacaoAnterior)
        {
            _logger.LogWarning(
                "Divergência detectada! Atrativo: {AtrativoId}, Ocupação DB: {Anterior}, Ocupação Real: {Real}. Corrigindo...",
                atrativoId, ocupacaoAnterior, ocupacaoReal);

            atrativo.OcupacaoAtual = Math.Min(ocupacaoReal, atrativo.CapacidadeMaxima);
            atrativo.UpdatedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Ocupação reconciliada. Atrativo: {AtrativoId}, De: {Anterior} Para: {Novo}",
                atrativoId, ocupacaoAnterior, atrativo.OcupacaoAtual);
        }
    }
}
