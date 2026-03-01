using EcoTurismo.Domain.Enums;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class CreateQuiosqueRequest
{
    public Guid? AtrativoId { get; set; }
    public int Numero { get; set; }
    public bool TemChurrasqueira { get; set; }
    public int Status { get; set; } = (int)QuiosqueStatus.Disponivel;
    public int PosicaoX { get; set; }
    public int PosicaoY { get; set; }
}
