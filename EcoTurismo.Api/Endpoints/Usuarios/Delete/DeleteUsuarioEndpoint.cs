using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Usuarios;

public class DeleteUsuarioRequest
{
    public Guid Id { get; set; }
}

public class DeleteUsuarioEndpoint : Endpoint<DeleteUsuarioRequest>
{
    private readonly IUsuarioService _service;

    public DeleteUsuarioEndpoint(IUsuarioService service) => _service = service;

    public override void Configure()
    {
        Delete("/api/usuarios/{Id}");
        Policies(RolePolicies.AdminPolicy);
    }

    public override async Task HandleAsync(DeleteUsuarioRequest req, CancellationToken ct)
    {
        var sucesso = await _service.ExcluirAsync(req.Id);

        if (!sucesso)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
