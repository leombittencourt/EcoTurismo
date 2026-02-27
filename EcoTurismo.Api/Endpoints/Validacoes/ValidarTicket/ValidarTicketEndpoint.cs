using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;
using System.Security.Claims;

namespace EcoTurismo.Api.Endpoints.Validacoes;

public class ValidarTicketEndpoint : Endpoint<ValidarTicketRequest, ValidarTicketResponse>
{
    private readonly IReservaService _service;

    public ValidarTicketEndpoint(IReservaService service) => _service = service;

    public override void Configure()
    {
        Post("/api/validacoes");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ValidarTicketRequest req, CancellationToken ct)
    {
        Guid? operadorId = null;
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is not null)
            operadorId = Guid.Parse(claim.Value);

        var result = await _service.ValidarTicketAsync(
            new ValidacaoRequest(req.Token, req.AtrativoId),
            operadorId
        );

        await Send.OkAsync(new ValidarTicketResponse
        {
            Valido = result.Valido,
            Mensagem = result.Mensagem,
            Reserva = result.Reserva
        }, ct);
    }
}
