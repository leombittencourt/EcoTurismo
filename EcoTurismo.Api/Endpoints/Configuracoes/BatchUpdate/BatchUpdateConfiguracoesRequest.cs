namespace EcoTurismo.Api.Endpoints.Configuracoes;

public class BatchUpdateConfiguracoesRequest
{
    public List<ConfigItem> Configs { get; set; } = [];
}

public class ConfigItem
{
    public string Chave { get; set; } = string.Empty;
    public string? Valor { get; set; }
}
