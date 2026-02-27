using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class GetQuiosqueRequest
{
    public Guid Id { get; set; }
}

public class GetQuiosqueEndpoint : Endpoint<GetQuiosqueRequest, QuiosqueDto>
{
    private readonly IQuiosqueService _service;

    public GetQuiosqueEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Get("/api/quiosques/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetQuiosqueRequest req, CancellationToken ct)
    {
        var list = await _service.ListarAsync(null);
        var quiosque = list.FirstOrDefault(q => q.Id == req.Id);

        if (quiosque is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(quiosque, ct);
    }
}
