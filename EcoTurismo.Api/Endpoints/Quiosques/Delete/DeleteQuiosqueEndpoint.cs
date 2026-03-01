using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class DeleteQuiosqueEndpoint : EndpointWithoutRequest
{
    private readonly IQuiosqueService _service;

    public DeleteQuiosqueEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Delete("/api/quiosques/{Id}");
        Policies(RolePolicies.AdminOrBalnearioPolicy); // Apenas Admin ou Balneário pode deletar quiosques
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");

        var ok = await _service.ExcluirAsync(id);

        if (!ok)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
