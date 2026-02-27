using EcoTurismo.Api.Authorization;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcoTurismo.Api.Endpoints.Profiles;

public class MeEndpoint : EndpointWithoutRequest<MeResponse>
{
    private readonly EcoTurismoDbContext _db;

    public MeEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/profiles/me");

        // Qualquer usuário autenticado pode ver seu próprio perfil
        Policies(RolePolicies.AnyAuthenticatedPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        // Buscar usuário SEM includes (evitar navegações)
        var usuario = await _db.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == Guid.Parse(userId))
            .Select(u => new
            {
                u.Id,
                u.Nome,
                u.Email,
                u.RoleId,
                u.MunicipioId,
                u.AtrativoId
            })
            .FirstOrDefaultAsync(ct);

        if (usuario is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        // Buscar nome da role separadamente
        var roleName = await _db.Roles
            .AsNoTracking()
            .Where(r => r.Id == usuario.RoleId)
            .Select(r => r.Name)
            .FirstOrDefaultAsync(ct);

        await Send.OkAsync(new MeResponse
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Role = roleName ?? "Unknown",
            MunicipioId = usuario.MunicipioId,
            AtrativoId = usuario.AtrativoId
        }, ct);
    }
}
