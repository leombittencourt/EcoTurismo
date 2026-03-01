namespace EcoTurismo.Domain.Enums;

/// <summary>
/// Status possíveis de um quiosque
/// </summary>
public enum QuiosqueStatus
{
    /// <summary>
    /// Quiosque disponível para reserva
    /// </summary>
    Disponivel = 1,

    /// <summary>
    /// Quiosque ocupado por reserva ativa
    /// </summary>
    Ocupado = 2,

    /// <summary>
    /// Quiosque em manutenção, indisponível temporariamente
    /// </summary>
    Manutencao = 3,

    /// <summary>
    /// Quiosque bloqueado pela administração
    /// </summary>
    Bloqueado = 4,

    /// <summary>
    /// Quiosque inativo (não está sendo usado)
    /// </summary>
    Inativo = 5
}
