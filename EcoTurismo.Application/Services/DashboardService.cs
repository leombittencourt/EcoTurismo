using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Application.Services;
public sealed class DashboardService : IDashboardService
{
    private static readonly ReservaStatus[] StatusValidos =
    [
        ReservaStatus.Confirmada,
        ReservaStatus.Validada,
        ReservaStatus.EmAndamento,
        ReservaStatus.Concluida
    ];

    private readonly EcoTurismoDbContext _db;

    public DashboardService(EcoTurismoDbContext db)
    {
        _db = db;
    }
    public async Task<DashboardDto> GetDashboardAsync(string periodo, CancellationToken ct = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var dias = periodo switch
        {
            "30d" => 30,
            "6m" => 180,
            _ => 7
        };

        var dataInicio = hoje.AddDays(-(dias - 1));
        var metadeJanela = dataInicio.AddDays(dias / 2);

        var reservasPeriodo = await _db.Reservas
            .Where(r => r.Data >= dataInicio && r.Data <= hoje)
            .Where(r => StatusValidos.Contains(r.Status))
            .Select(r => new
            {
                r.AtrativoId,
                r.Data,
                r.DataFim,
                r.QuantidadePessoas,
                r.UfOrigem
            })
            .ToListAsync(ct);

        var visitantesHoje = reservasPeriodo
            .Where(r => r.Data == hoje)
            .Sum(r => r.QuantidadePessoas);

        var visitantesOntem = reservasPeriodo
            .Where(r => r.Data == hoje.AddDays(-1))
            .Sum(r => r.QuantidadePessoas);

        var visitantesTendencia = GetTrendSimple(visitantesHoje, visitantesOntem);

        // Frontend shows "h", so average stay is in hours.
        var permanenciaHoras = reservasPeriodo
            .Select(r =>
            {
                if (r.DataFim.HasValue)
                {
                    var diasCamping = r.DataFim.Value.DayNumber - r.Data.DayNumber;
                    var diasMinimo = Math.Max(diasCamping, 1);
                    return diasMinimo * 24;
                }

                return 8;
            })
            .ToList();

        var permanenciaMedia = permanenciaHoras.Count > 0
            ? Math.Round(permanenciaHoras.Average(), 1)
            : 0;

        var atrativosAtivos = await _db.Atrativos
            .Where(a => a.Status == "ativo")
            .Select(a => new
            {
                a.Id,
                a.Nome,
                a.OcupacaoAtual,
                a.CapacidadeMaxima
            })
            .ToListAsync(ct);

        var ocupacaoMedia = atrativosAtivos.Count > 0
            ? Math.Round(
                atrativosAtivos.Average(a =>
                    a.CapacidadeMaxima > 0
                        ? (double)a.OcupacaoAtual / a.CapacidadeMaxima * 100
                        : 0
                ),
                1
            )
            : 0;

        var pressaoTuristica = ocupacaoMedia switch
        {
            < 40 => "baixa",
            < 65 => "moderada",
            < 85 => "alta",
            _ => "critica"
        };

        var visitantesPorDia = reservasPeriodo
            .GroupBy(r => r.Data)
            .OrderBy(g => g.Key)
            .Select(g => new DataPointDto(
                Label: FormatLabel(g.Key, periodo),
                Valor: g.Sum(x => x.QuantidadePessoas)
            ))
            .ToList();

        var ocupacaoPorBalneario = atrativosAtivos
            .OrderByDescending(a => a.OcupacaoAtual)
            .Take(10)
            .Select(a => new OcupacaoBalnearioDto(
                Nome: a.Nome,
                Ocupacao: a.OcupacaoAtual,
                Capacidade: a.CapacidadeMaxima
            ))
            .ToList();

        var origemPorUf = reservasPeriodo
            .GroupBy(r => string.IsNullOrWhiteSpace(r.UfOrigem) ? "N/A" : r.UfOrigem.ToUpperInvariant())
            .Select(g => new OrigemUfDto(
                Uf: g.Key,
                Quantidade: g.Sum(x => x.QuantidadePessoas)
            ))
            .OrderByDescending(x => x.Quantidade)
            .Take(10)
            .ToList();

        var inicioEvolucao = new DateOnly(hoje.Year, hoje.Month, 1).AddMonths(-5);
        var reservasEvolucao = await _db.Reservas
            .Where(r => r.Data >= inicioEvolucao && r.Data <= hoje)
            .Where(r => StatusValidos.Contains(r.Status))
            .Select(r => new { r.Data, r.QuantidadePessoas })
            .ToListAsync(ct);

        var evolucaoMensal = reservasEvolucao
            .GroupBy(r => new { r.Data.Year, r.Data.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new DataPointDto(
                Label: $"{g.Key.Month:D2}/{g.Key.Year}",
                Valor: g.Sum(x => x.QuantidadePessoas)
            ))
            .ToList();

        var nomesAtrativos = atrativosAtivos.ToDictionary(a => a.Id, a => a.Nome);
        var topAtrativos = reservasPeriodo
            .GroupBy(r => r.AtrativoId)
            .Select(g =>
            {
                var total = g.Sum(x => x.QuantidadePessoas);
                var anterior = g.Where(x => x.Data < metadeJanela).Sum(x => x.QuantidadePessoas);
                var atual = g.Where(x => x.Data >= metadeJanela).Sum(x => x.QuantidadePessoas);
                var tendencia = GetTrend(current: atual, previous: anterior);

                var nome = nomesAtrativos.TryGetValue(g.Key, out var nomeAtrativo)
                    ? nomeAtrativo
                    : "Atrativo";

                return new TopAtrativoDto(
                    Nome: nome,
                    Visitantes: total,
                    Tendencia: tendencia
                );
            })
            .OrderByDescending(x => x.Visitantes)
            .Take(5)
            .ToList();

        return new DashboardDto(
            VisitantesHoje: visitantesHoje,
            VisitantesTendencia: visitantesTendencia,
            PermanenciaMedia: permanenciaMedia,
            OcupacaoMedia: ocupacaoMedia,
            PressaoTuristica: pressaoTuristica,
            VisitantesPorDia: visitantesPorDia,
            OcupacaoPorBalneario: ocupacaoPorBalneario,
            OrigemPorUF: origemPorUf,
            EvolucaoMensal: evolucaoMensal,
            TopAtrativos: topAtrativos
        );
    }

    private static string GetTrendSimple(int current, int previous)
    {
        if (previous == 0)
            return current > 0 ? "up" : "stable";

        if (current > previous)
            return "up";

        if (current < previous)
            return "down";

        return "stable";
    }

    private static string GetTrend(int current, int previous)
    {
        if (previous == 0)
            return current > 0 ? "up" : "stable";

        if (current > previous * 1.05)
            return "up";

        if (current < previous * 0.95)
            return "down";

        return "stable";
    }

    private static string FormatLabel(DateOnly date, string periodo)
    {
        return periodo switch
        {
            "6m" => $"{date.Month:D2}/{date.Year}",
            "30d" => $"{date.Day:D2}/{date.Month:D2}",
            _ => $"{date.Day:D2}/{date.Month:D2}"
        };
    }
}