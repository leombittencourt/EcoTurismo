using EcoTurismo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EcoTurismo.Api.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionAuthorizationHandler> _logger;

    public PermissionAuthorizationHandler(
        IPermissionService permissionService,
        ILogger<PermissionAuthorizationHandler> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        _logger.LogInformation("🔐 Verificando permissões. Usuário: {User}", context.User.Identity?.Name);

        var roleIdClaim = context.User.FindFirst("role_id");
        if (roleIdClaim == null)
        {
            _logger.LogWarning("❌ Claim 'role_id' não encontrada no token");
            _logger.LogInformation("Claims disponíveis: {Claims}", 
                string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}")));
            return;
        }

        if (!Guid.TryParse(roleIdClaim.Value, out var roleId))
        {
            _logger.LogWarning("❌ role_id inválido: {RoleId}", roleIdClaim.Value);
            return;
        }

        _logger.LogInformation("📋 Permissões necessárias: {Permissions}", 
            string.Join(", ", requirement.Permissions));

        // Verifica se o usuário tem pelo menos uma das permissões necessárias
        var hasPermission = await _permissionService.HasAnyPermissionAsync(roleId, requirement.Permissions);

        if (hasPermission)
        {
            _logger.LogInformation("✅ Autorizado! Usuário tem permissão");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("❌ Negado! Usuário não tem nenhuma das permissões necessárias");

            // Log das permissões do usuário
            var userPermissions = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
            _logger.LogInformation("Permissões do usuário: {UserPermissions}", 
                string.Join(", ", userPermissions));
        }
    }
}
