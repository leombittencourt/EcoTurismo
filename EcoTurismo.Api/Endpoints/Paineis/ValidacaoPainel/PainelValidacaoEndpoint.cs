using EcoTurismo.Api.Authorization;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Paineis;

/// <summary>
/// Endpoint de métricas do painel de validação
/// Retorna estatísticas de reservas e validações para um atrativo em uma data específica
/// </summary>
public class PainelValidacaoEndpoint : Endpoint<PainelValidacaoRequest, PainelValidacaoResponse>
{
    private readonly EcoTurismoDbContext _db;

    public PainelValidacaoEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/painel-validacao");
        Policies(RolePolicies.AdminOrBalnearioPolicy); // Admin ou Balneário podem ver métricas
    }

    public override async Task HandleAsync(PainelValidacaoRequest req, CancellationToken ct)
    {
        // Se não especificou data, usa hoje
        var data = req.Data ?? DateOnly.FromDateTime(DateTime.Today);

        // Buscar atrativo
        var atrativo = await _db.Atrativos
            .Where(a => a.Id == req.AtrativoId)
            .Select(a => new { a.Id, a.Nome, a.CapacidadeMaxima })
            .FirstOrDefaultAsync(ct);

        if (atrativo == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        // Buscar todas as reservas do dia
        var reservasDoDia = await _db.Reservas
            .Where(r => r.AtrativoId == req.AtrativoId && r.Data == data)
            .Select(r => new
            {
                r.Id,
                r.NomeVisitante,
                r.Email,
                r.QuantidadePessoas,
                r.Status,
                r.Token,
                r.Tipo,
                r.QuiosqueId,
                NumeroQuiosque = r.Quiosque != null ? r.Quiosque.Numero : (int?)null
            })
            .ToListAsync(ct);

        // Calcular métricas usando o enum
        var validadas = reservasDoDia.Count(r => 
            r.Status == ReservaStatus.Validada || 
            r.Status == ReservaStatus.Concluida || 
            r.Status == ReservaStatus.EmAndamento);
        var recusadas = reservasDoDia.Count(r => r.Status == ReservaStatus.Cancelada);
        var pendentes = reservasDoDia.Count(r => 
            r.Status == ReservaStatus.Confirmada || 
            r.Status == ReservaStatus.EmAndamento || 
            r.Status == ReservaStatus.Validada);

        // Ocupação atual considera apenas reservas ativas
        var ocupacaoAtual = reservasDoDia
            .Where(r => r.Status == ReservaStatus.Confirmada || 
                       r.Status == ReservaStatus.EmAndamento || 
                       r.Status == ReservaStatus.Validada)
            .Sum(r => r.QuantidadePessoas);

        var percentualOcupacao = atrativo.CapacidadeMaxima > 0
            ? Math.Round((decimal)ocupacaoAtual / atrativo.CapacidadeMaxima * 100, 2)
            : 0;

        var response = new PainelValidacaoResponse
        {
            AtrativoId = atrativo.Id,
            NomeAtrativo = atrativo.Nome,
            Data = data,
            Validadas = validadas,
            Recusadas = recusadas,
            Pendentes = pendentes,
            OcupacaoAtual = ocupacaoAtual,
            CapacidadeMaxima = atrativo.CapacidadeMaxima,
            PercentualOcupacao = percentualOcupacao,
            TotalReservasDia = reservasDoDia.Count
        };

        // Incluir lista detalhada de reservas se solicitado
        if (req.IncluirReservas)
        {
            response.ReservasDoDia = reservasDoDia
                .OrderBy(r => r.NomeVisitante)
                .Select(r => new ReservaDoDiaDto
                {
                    Id = r.Id,
                    NomeVisitante = r.NomeVisitante,
                    Email = r.Email,
                    QuantidadePessoas = r.QuantidadePessoas,
                    Status = r.Status.ToStringValue(),
                    StatusDescricao = r.Status.ToDescricao(),
                    Token = r.Token,
                    Tipo = r.Tipo,
                    QuiosqueId = r.QuiosqueId,
                    NumeroQuiosque = r.NumeroQuiosque
                })
                .ToList();
        }

        await Send.OkAsync(response, ct);
    }
}
