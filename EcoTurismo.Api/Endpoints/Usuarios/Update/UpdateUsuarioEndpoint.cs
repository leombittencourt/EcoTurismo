using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Usuarios;

public class UpdateUsuarioRequest
{
    public Guid Id { get; init; }
    public string? Nome { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
    public Guid? RoleId { get; init; }
    public Guid? MunicipioId { get; init; }
    public Guid? AtrativoId { get; init; }
    public string? Telefone { get; init; }
    public string? Cpf { get; init; }
    public bool? Ativo { get; init; }
}

public class UpdateUsuarioEndpoint : Endpoint<UpdateUsuarioRequest, UsuarioDto>
{
    private readonly IUsuarioService _service;

    public UpdateUsuarioEndpoint(IUsuarioService service) => _service = service;

    public override void Configure()
    {
        Put("/api/usuarios/{Id}");
        Policies(RolePolicies.AdminPolicy);
    }

    public override async Task HandleAsync(UpdateUsuarioRequest req, CancellationToken ct)
    {
        try
        {
            var updateRequest = new UsuarioUpdateRequest
            {
                Nome = req.Nome,
                Email = req.Email,
                Password = req.Password,
                RoleId = req.RoleId,
                MunicipioId = req.MunicipioId,
                AtrativoId = req.AtrativoId,
                Telefone = req.Telefone,
                Cpf = req.Cpf,
                Ativo = req.Ativo
            };

            var usuario = await _service.AtualizarAsync(req.Id, updateRequest);

            if (usuario is null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            await Send.OkAsync(usuario, ct);
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message);
        }
    }
}
