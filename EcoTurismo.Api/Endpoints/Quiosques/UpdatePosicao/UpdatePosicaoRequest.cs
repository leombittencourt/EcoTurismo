namespace EcoTurismo.Api.Endpoints.Quiosques;

public class UpdatePosicaoRequest
{
    public Guid Id { get; set; }
    public int PosicaoX { get; set; }
    public int PosicaoY { get; set; }
}
