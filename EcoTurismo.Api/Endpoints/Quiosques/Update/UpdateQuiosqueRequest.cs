namespace EcoTurismo.Api.Endpoints.Quiosques;

public class UpdateQuiosqueRequest
{
    public Guid Id { get; set; }
    public int? Numero { get; set; }
    public bool? TemChurrasqueira { get; set; }
    public string? Status { get; set; }
    public int? PosicaoX { get; set; }
    public int? PosicaoY { get; set; }
}
