using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Usuarios;

public class GetUsuarioRequest
{
    public Guid Id { get; set; }
}

public class GetUsuarioEndpoint : Endpoint<GetUsuarioRequest, UsuarioDto>
{
    private readonly IUsuarioService _service;

    public GetUsuarioEndpoint(IUsuarioService service) => _service = service;

    public override void Configure()
    {
        Get("/api/usuarios/{Id}");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
    }

    public override async Task HandleAsync(GetUsuarioRequest req, CancellationToken ct)
    {
        var usuario = await _service.ObterPorIdAsync(req.Id);

        if (usuario is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(usuario, ct);
    }
}
