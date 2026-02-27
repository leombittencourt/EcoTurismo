using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EcoTurismo.Api.Authorization;

public class RoleRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; }

    public RoleRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly ILogger<RoleAuthorizationHandler> _logger;

    public RoleAuthorizationHandler(ILogger<RoleAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        var userName = context.User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("🔐 Verificando role. Usuário: {User}", userName);

        // Buscar role do token (pode estar em "role" ou em ClaimTypes.Role)
        var roleClaim = context.User.FindFirst(ClaimTypes.Role) 
            ?? context.User.FindFirst("role") 
            ?? context.User.FindFirst("role_name");

        if (roleClaim == null)
        {
            _logger.LogWarning("❌ Claim de role não encontrada no token");
            _logger.LogInformation("Claims disponíveis: {Claims}",
                string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}")));
            return Task.CompletedTask;
        }

        var userRole = roleClaim.Value;
        _logger.LogInformation("👤 Role do usuário: {Role}", userRole);
        _logger.LogInformation("🎯 Roles permitidas: {AllowedRoles}", 
            string.Join(", ", requirement.AllowedRoles));

        // Verificar se a role do usuário está nas roles permitidas (case insensitive)
        if (requirement.AllowedRoles.Any(r => 
            string.Equals(r, userRole, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogInformation("✅ Autorizado! Role '{Role}' é permitida", userRole);
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("❌ Negado! Role '{UserRole}' não está nas roles permitidas: {AllowedRoles}",
                userRole, string.Join(", ", requirement.AllowedRoles));
        }

        return Task.CompletedTask;
    }
}
