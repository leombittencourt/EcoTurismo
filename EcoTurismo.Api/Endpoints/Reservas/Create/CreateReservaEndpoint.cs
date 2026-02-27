using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;
using AuthDomain = EcoTurismo.Domain.Authorization;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class CreateReservaEndpoint : Endpoint<CreateReservaRequest, ReservaDto>
{
    private readonly IReservaService _service;

    public CreateReservaEndpoint(IReservaService service) => _service = service;

    public override void Configure()
    {
        Post("/api/reservas");
        Permissions(AuthDomain.Permissions.ReservasCreate);
    }

    public override async Task HandleAsync(CreateReservaRequest req, CancellationToken ct)
    {
        var dto = new ReservaCreateRequest
        {
            AtrativoId = req.AtrativoId,
            QuiosqueId = req.QuiosqueId,
            NomeVisitante = req.NomeVisitante,
            Email = req.Email,
            Cpf = req.Cpf,
            CidadeOrigem = req.CidadeOrigem,
            UfOrigem = req.UfOrigem,
            Tipo = req.Tipo,
            Data = req.Data,
            DataFim = req.DataFim,
            QuantidadePessoas = req.QuantidadePessoas,
        };

        var reserva = await _service.CriarAsync(dto);
        await Send.CreatedAtAsync<GetReservaEndpoint>(new { id = reserva.Id }, reserva, cancellation: ct);
    }
}
