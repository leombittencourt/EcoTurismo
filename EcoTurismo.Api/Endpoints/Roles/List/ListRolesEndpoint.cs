using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Roles.List;

public class ListRolesEndpoint : EndpointWithoutRequest<List<RoleDto>>
{
    private readonly EcoTurismoDbContext _db;

    public ListRolesEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/roles");
        Policies(RolePolicies.AdminPolicy);
        Description(d => d
            .WithTags("Roles")
            .WithSummary("Lista todas as roles do sistema")
            .WithDescription("Retorna uma lista de todas as roles cadastradas")
            .Produces<List<RoleDto>>(200)
            .Produces(401)
            .Produces(403));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var roles = await _db.Roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto(
                r.Id,
                r.Name,
                r.Description,
                r.IsActive
            ))
            .ToListAsync(ct);

        await Send.OkAsync(roles, ct);
    }
}
