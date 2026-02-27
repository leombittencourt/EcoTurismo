namespace EcoTurismo.Application.DTOs;

public record ReservaDto(
    Guid Id,
    Guid AtrativoId,
    Guid? QuiosqueId,
    string NomeVisitante,
    string Email,
    string Cpf,
    string CidadeOrigem,
    string UfOrigem,
    string Tipo,
    DateOnly Data,
    DateOnly? DataFim,
    int QuantidadePessoas,
    string Status,
    string Token,
    DateTimeOffset CreatedAt
);

public record ReservaCreateRequest
{
    public Guid AtrativoId { get; init; }
    public Guid? QuiosqueId { get; init; }
    public string NomeVisitante { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public string CidadeOrigem { get; init; } = string.Empty;
    public string UfOrigem { get; init; } = string.Empty;
    public string Tipo { get; init; } = "day_use";
    public DateOnly Data { get; init; }
    public DateOnly? DataFim { get; init; }
    public int QuantidadePessoas { get; init; } = 1;
}

public record ReservaStatusUpdateRequest(string Status);
