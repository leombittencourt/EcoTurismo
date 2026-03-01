namespace EcoTurismo.Domain.Enums;

public static class QuiosqueStatusExtensions
{
    /// <summary>
    /// Retorna o nome do status em português
    /// </summary>
    public static string ToDescricao(this QuiosqueStatus status)
    {
        return status switch
        {
            QuiosqueStatus.Disponivel => "Disponível",
            QuiosqueStatus.Ocupado => "Ocupado",
            QuiosqueStatus.Manutencao => "Em Manutenção",
            QuiosqueStatus.Bloqueado => "Bloqueado",
            QuiosqueStatus.Inativo => "Inativo",
            _ => status.ToString()
        };
    }

    /// <summary>
    /// Retorna o valor string para serialização
    /// </summary>
    public static string ToStringValue(this QuiosqueStatus status)
    {
        return status switch
        {
            QuiosqueStatus.Disponivel => "disponivel",
            QuiosqueStatus.Ocupado => "ocupado",
            QuiosqueStatus.Manutencao => "manutencao",
            QuiosqueStatus.Bloqueado => "bloqueado",
            QuiosqueStatus.Inativo => "inativo",
            _ => status.ToString().ToLower()
        };
    }

    /// <summary>
    /// Converte string para enum
    /// </summary>
    public static QuiosqueStatus FromString(string status)
    {
        return status?.ToLower() switch
        {
            "disponivel" => QuiosqueStatus.Disponivel,
            "ocupado" => QuiosqueStatus.Ocupado,
            "manutencao" => QuiosqueStatus.Manutencao,
            "bloqueado" => QuiosqueStatus.Bloqueado,
            "inativo" => QuiosqueStatus.Inativo,
            _ => QuiosqueStatus.Disponivel // Default
        };
    }

    /// <summary>
    /// Verifica se o quiosque está disponível para reserva
    /// </summary>
    public static bool EstaDisponivel(this QuiosqueStatus status)
    {
        return status == QuiosqueStatus.Disponivel;
    }

    /// <summary>
    /// Verifica se o quiosque pode ser reservado
    /// </summary>
    public static bool PodeSerReservado(this QuiosqueStatus status)
    {
        return status is QuiosqueStatus.Disponivel or QuiosqueStatus.Ocupado;
    }

    /// <summary>
    /// Retorna cor para UI
    /// </summary>
    public static string ToCor(this QuiosqueStatus status)
    {
        return status switch
        {
            QuiosqueStatus.Disponivel => "green",
            QuiosqueStatus.Ocupado => "red",
            QuiosqueStatus.Manutencao => "orange",
            QuiosqueStatus.Bloqueado => "gray",
            QuiosqueStatus.Inativo => "lightgray",
            _ => "gray"
        };
    }

    /// <summary>
    /// Retorna ícone para UI
    /// </summary>
    public static string ToIcone(this QuiosqueStatus status)
    {
        return status switch
        {
            QuiosqueStatus.Disponivel => "✅",
            QuiosqueStatus.Ocupado => "🔴",
            QuiosqueStatus.Manutencao => "🔧",
            QuiosqueStatus.Bloqueado => "🔒",
            QuiosqueStatus.Inativo => "⚫",
            _ => "❓"
        };
    }
}
