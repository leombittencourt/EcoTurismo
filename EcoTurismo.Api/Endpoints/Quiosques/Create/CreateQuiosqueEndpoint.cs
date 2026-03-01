using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class CreateQuiosqueEndpoint : Endpoint<CreateQuiosqueRequest, QuiosqueDto>
{
    private readonly IQuiosqueService _service;

    public CreateQuiosqueEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Post("/api/quiosques");
        Policies(RolePolicies.AdminOrBalnearioPolicy);
    }

    public override async Task HandleAsync(CreateQuiosqueRequest req, CancellationToken ct)
    {
        var dto = new QuiosqueCreateRequest
        {
            AtrativoId = req.AtrativoId,
            Numero = req.Numero,
            TemChurrasqueira = req.TemChurrasqueira,
            Status = req.Status,
            PosicaoX = req.PosicaoX,
            PosicaoY = req.PosicaoY,
        };

        var result = await _service.CriarAsync(dto);
        await Send.CreatedAtAsync<GetQuiosqueEndpoint>(new { id = result.Id }, result, cancellation: ct);
    }
}
