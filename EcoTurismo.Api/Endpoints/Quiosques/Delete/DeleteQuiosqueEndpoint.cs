using EcoTurismo.Application.Interfaces;
using FastEndpoints;
using AuthDomain = EcoTurismo.Domain.Authorization;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class DeleteQuiosqueEndpoint : Endpoint<DeleteQuiosqueRequest>
{
    private readonly IQuiosqueService _service;

    public DeleteQuiosqueEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Delete("/api/quiosques/{Id}");
        Permissions(AuthDomain.Permissions.QuiosquesDelete);
    }

    public override async Task HandleAsync(DeleteQuiosqueRequest req, CancellationToken ct)
    {
        var ok = await _service.ExcluirAsync(req.Id);

        if (!ok)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
