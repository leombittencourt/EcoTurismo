namespace EcoTurismo.Application.DTOs;

public record ConfiguracaoDto(string Chave, string? Valor);

public record ConfiguracaoUpdateItem(string Chave, string? Valor);

public record ConfiguracaoBatchUpdateRequest(List<ConfiguracaoUpdateItem> Configs);
