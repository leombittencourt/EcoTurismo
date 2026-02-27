using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Usuarios;

public class CreateUsuarioEndpoint : Endpoint<UsuarioCreateRequest, UsuarioDto>
{
    private readonly IUsuarioService _service;

    public CreateUsuarioEndpoint(IUsuarioService service) => _service = service;

    public override void Configure()
    {
        Post("/api/usuarios");
        Policies(RolePolicies.AdminPolicy); // Apenas Admin pode criar usuários
    }

    public override async Task HandleAsync(UsuarioCreateRequest req, CancellationToken ct)
    {
        try
        {
            var usuario = await _service.CriarAsync(req);
            await Send.CreatedAtAsync<GetUsuarioEndpoint>(
                new { id = usuario.Id },
                usuario,
                cancellation: ct
            );
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message);
        }
    }
}
