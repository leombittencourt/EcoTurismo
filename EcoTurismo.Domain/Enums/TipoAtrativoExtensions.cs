namespace EcoTurismo.Domain.Enums;

public static class TipoAtrativoExtensions
{
    public static string ToStringValue(this TipoAtrativo tipo)
    {
        return tipo switch
        {
            TipoAtrativo.Balneario => "balneario",
            TipoAtrativo.Cachoeira => "cachoeira",
            TipoAtrativo.Trilha => "trilha",
            TipoAtrativo.Parque => "parque",
            TipoAtrativo.FazendaEcoturismo => "fazenda_ecoturismo",
            _ => "balneario"
        };
    }

    public static TipoAtrativo FromString(string tipo)
    {
        return tipo.ToLowerInvariant() switch
        {
            "balneario" or "balneário" => TipoAtrativo.Balneario,
            "cachoeira" => TipoAtrativo.Cachoeira,
            "trilha" => TipoAtrativo.Trilha,
            "parque" => TipoAtrativo.Parque,
            "fazenda_ecoturismo" or "fazenda ecoturismo" or "fazendaecoturismo" => TipoAtrativo.FazendaEcoturismo,
            _ => TipoAtrativo.Balneario
        };
    }

    public static string ToDescricao(this TipoAtrativo tipo)
    {
        return tipo switch
        {
            TipoAtrativo.Balneario => "Balneário",
            TipoAtrativo.Cachoeira => "Cachoeira",
            TipoAtrativo.Trilha => "Trilha",
            TipoAtrativo.Parque => "Parque",
            TipoAtrativo.FazendaEcoturismo => "Fazenda Ecoturismo",
            _ => "Balneário"
        };
    }
}
