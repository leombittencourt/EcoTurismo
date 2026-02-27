using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class DeleteQuiosqueEndpoint : Endpoint<DeleteQuiosqueRequest>
{
    private readonly IQuiosqueService _service;

    public DeleteQuiosqueEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Delete("/api/quiosques/{Id}");
        Policies(RolePolicies.AdminPolicy); // Apenas Admin pode deletar quiosques
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
