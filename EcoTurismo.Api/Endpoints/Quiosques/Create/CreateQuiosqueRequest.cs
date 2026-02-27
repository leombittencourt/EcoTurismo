namespace EcoTurismo.Api.Endpoints.Quiosques;

public class CreateQuiosqueRequest
{
    public Guid? AtrativoId { get; set; }
    public int Numero { get; set; }
    public bool TemChurrasqueira { get; set; }
    public string Status { get; set; } = "disponivel";
    public int PosicaoX { get; set; }
    public int PosicaoY { get; set; }
}
