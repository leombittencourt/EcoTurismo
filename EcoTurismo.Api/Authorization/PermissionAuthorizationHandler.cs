using EcoTurismo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EcoTurismo.Api.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var roleIdClaim = context.User.FindFirst("role_id");
        if (roleIdClaim == null || !Guid.TryParse(roleIdClaim.Value, out var roleId))
        {
            return;
        }

        // Verifica se o usuário tem pelo menos uma das permissões necessárias
        if (await _permissionService.HasAnyPermissionAsync(roleId, requirement.Permissions))
        {
            context.Succeed(requirement);
        }
    }
}
