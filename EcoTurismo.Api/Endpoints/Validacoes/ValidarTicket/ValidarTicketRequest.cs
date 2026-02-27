namespace EcoTurismo.Api.Endpoints.Validacoes;

public class ValidarTicketRequest
{
    public string Token { get; set; } = string.Empty;
    public Guid? AtrativoId { get; set; }
}
