namespace EcoTurismo.Application.Interfaces;

/// <summary>
/// Serviço responsável por gerenciar a ocupação de atrativos
/// </summary>
public interface IOcupacaoService
{
    /// <summary>
    /// Incrementa ocupação do atrativo para uma data específica
    /// </summary>
    Task<bool> IncrementarOcupacaoAsync(Guid atrativoId, DateOnly data, int quantidade, CancellationToken ct = default);

    /// <summary>
    /// Decrementa ocupação do atrativo para uma data específica
    /// </summary>
    Task<bool> DecrementarOcupacaoAsync(Guid atrativoId, DateOnly data, int quantidade, CancellationToken ct = default);

    /// <summary>
    /// Recalcula ocupação de todos os atrativos baseado nas reservas ativas
    /// </summary>
    Task ReconciliarOcupacoesAsync(CancellationToken ct = default);

    /// <summary>
    /// Recalcula ocupação de um atrativo específico
    /// </summary>
    Task ReconciliarOcupacaoAtrativoAsync(Guid atrativoId, CancellationToken ct = default);
}
