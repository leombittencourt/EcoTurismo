using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Usuarios;

public class ListUsuariosEndpoint : EndpointWithoutRequest<List<UsuarioListItem>>
{
    private readonly IUsuarioService _service;

    public ListUsuariosEndpoint(IUsuarioService service) => _service = service;

    public override void Configure()
    {
        Get("/api/usuarios");
        Policies(RolePolicies.AdminPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var usuarios = await _service.ListarAsync();
        await Send.OkAsync(usuarios, ct);
    }
}
