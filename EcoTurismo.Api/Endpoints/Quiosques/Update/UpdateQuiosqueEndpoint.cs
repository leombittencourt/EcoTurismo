using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;
using AuthDomain = EcoTurismo.Domain.Authorization;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class UpdateQuiosqueEndpoint : Endpoint<UpdateQuiosqueRequest, QuiosqueDto>
{
    private readonly IQuiosqueService _service;

    public UpdateQuiosqueEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Put("/api/quiosques/{Id}");
        Permissions(AuthDomain.Permissions.QuiosquesUpdate);
    }

    public override async Task HandleAsync(UpdateQuiosqueRequest req, CancellationToken ct)
    {
        var dto = new QuiosqueUpdateRequest
        {
            Numero = req.Numero,
            TemChurrasqueira = req.TemChurrasqueira,
            Status = req.Status,
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
