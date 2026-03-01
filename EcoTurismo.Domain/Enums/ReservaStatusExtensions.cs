namespace EcoTurismo.Domain.Enums;

public static class ReservaStatusExtensions
{
    /// <summary>
    /// Retorna o nome do status em português
    /// </summary>
    public static string ToDescricao(this ReservaStatus status)
    {
        return status switch
        {
            ReservaStatus.Confirmada => "Confirmada",
            ReservaStatus.EmAndamento => "Em Andamento",
            ReservaStatus.Concluida => "Concluída",
            ReservaStatus.Cancelada => "Cancelada",
            ReservaStatus.Validada => "Validada",
            ReservaStatus.NaoCompareceu => "Não Compareceu",
            _ => status.ToString()
        };
    }

    /// <summary>
    /// Retorna o valor string para serialização
    /// </summary>
    public static string ToStringValue(this ReservaStatus status)
    {
        return status switch
        {
            ReservaStatus.Confirmada => "confirmada",
            ReservaStatus.EmAndamento => "em_andamento",
            ReservaStatus.Concluida => "concluida",
            ReservaStatus.Cancelada => "cancelada",
            ReservaStatus.Validada => "validada",
            ReservaStatus.NaoCompareceu => "nao_compareceu",
            _ => status.ToString().ToLower()
        };
    }

    /// <summary>
    /// Converte string para enum
    /// </summary>
    public static ReservaStatus FromString(string status)
    {
        return status?.ToLower() switch
        {
            "confirmada" => ReservaStatus.Confirmada,
            "em_andamento" => ReservaStatus.EmAndamento,
            "concluida" => ReservaStatus.Concluida,
            "cancelada" => ReservaStatus.Cancelada,
            "validada" => ReservaStatus.Validada,
            "nao_compareceu" => ReservaStatus.NaoCompareceu,
            _ => ReservaStatus.Confirmada // Default
        };
    }

    /// <summary>
    /// Verifica se a reserva está ativa (conta para ocupação)
    /// </summary>
    public static bool EstaAtiva(this ReservaStatus status)
    {
        return status is ReservaStatus.Confirmada or ReservaStatus.EmAndamento or ReservaStatus.Validada;
    }

    /// <summary>
    /// Verifica se a reserva foi validada/concluída
    /// </summary>
    public static bool FoiValidada(this ReservaStatus status)
    {
        return status is ReservaStatus.Validada or ReservaStatus.Concluida or ReservaStatus.EmAndamento;
    }

    /// <summary>
    /// Retorna cor para UI
    /// </summary>
    public static string ToCor(this ReservaStatus status)
    {
        return status switch
        {
            ReservaStatus.Confirmada => "blue",
            ReservaStatus.EmAndamento => "orange",
            ReservaStatus.Concluida => "green",
            ReservaStatus.Cancelada => "red",
            ReservaStatus.Validada => "green",
            ReservaStatus.NaoCompareceu => "gray",
            _ => "gray"
        };
    }
}
