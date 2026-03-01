namespace EcoTurismo.Domain.Enums;

/// <summary>
/// Status possíveis de uma reserva
/// </summary>
public enum ReservaStatus
{
    /// <summary>
    /// Reserva criada e confirmada, aguardando validação de entrada
    /// </summary>
    Confirmada = 1,

    /// <summary>
    /// Visitante já validou entrada e está no local
    /// </summary>
    EmAndamento = 2,

    /// <summary>
    /// Visita concluída, visitante saiu do local
    /// </summary>
    Concluida = 3,

    /// <summary>
    /// Reserva cancelada (pelo sistema ou usuário)
    /// </summary>
    Cancelada = 4,

    /// <summary>
    /// Reserva validada manualmente pelo operador
    /// </summary>
    Validada = 5,

    /// <summary>
    /// Reserva não compareceu (no-show)
    /// </summary>
    NaoCompareceu = 6
}
