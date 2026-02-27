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

        // Usar política baseada em role (Admin ou Prefeitura podem listar usuários)
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var usuarios = await _service.ListarAsync();
        await Send.OkAsync(usuarios, ct);
    }
}
