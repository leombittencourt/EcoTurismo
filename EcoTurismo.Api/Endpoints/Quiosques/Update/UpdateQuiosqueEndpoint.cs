using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class UpdateQuiosqueEndpoint : Endpoint<UpdateQuiosqueRequest, QuiosqueDto>
{
    private readonly IQuiosqueService _service;

    public UpdateQuiosqueEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Put("/api/quiosques/{Id}");
        AllowAnonymous();
        //Policies(RolePolicies.AdminOrBalnearioPolicy);
    }

    public override async Task HandleAsync(UpdateQuiosqueRequest req, CancellationToken ct)
    {
        var dto = new QuiosqueUpdateRequest
        {
            Numero = req.Numero,
            TemChurrasqueira = req.TemChurrasqueira,
            Status = req.Status.GetValueOrDefault(),
            PosicaoX = req.PosicaoX,
            PosicaoY = req.PosicaoY,
        };

        var result = await _service.AtualizarAsync(req.Id, dto);

        if (result is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(result, ct);
    }
}
