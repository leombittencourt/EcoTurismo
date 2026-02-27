using EcoTurismo.Domain.Entities;

namespace EcoTurismo.Application.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<string>> GetPermissionsByRoleIdAsync(Guid roleId);
    Task<IEnumerable<string>> GetPermissionsByRoleNameAsync(string roleName);
    Task<bool> HasPermissionAsync(Guid roleId, string permissionName);
    Task<bool> HasAnyPermissionAsync(Guid roleId, params string[] permissionNames);
    Task<Role?> GetRoleByNameAsync(string roleName);
    Task<Role?> GetRoleByIdAsync(Guid roleId);
}
