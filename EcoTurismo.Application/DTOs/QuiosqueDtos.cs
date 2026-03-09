using EcoTurismo.Domain.Enums;

namespace EcoTurismo.Application.DTOs;

public record QuiosqueDto(
    Guid Id,
    Guid? AtrativoId,
    int Numero,
    bool TemChurrasqueira,
    string Status,
    int PosicaoX,
    int PosicaoY
);

public record QuiosqueCreateRequest
{
    public Guid? AtrativoId { get; init; }
    public int Numero { get; init; }
    public bool TemChurrasqueira { get; init; }
    public int Status { get; init; } = (int)QuiosqueStatus.Disponivel;
    public int PosicaoX { get; init; }
    public int PosicaoY { get; init; }
}

public record QuiosqueUpdateRequest
{
    public int? Numero { get; init; }
    public bool? TemChurrasqueira { get; init; }
    public int? Status { get; init; }
    public int? PosicaoX { get; init; }
    public int? PosicaoY { get; init; }
}
