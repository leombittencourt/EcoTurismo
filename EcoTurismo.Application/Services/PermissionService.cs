using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EcoTurismo.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly EcoTurismoDbContext _db;
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "Permissions_";
    private const int CacheMinutes = 60;

    public PermissionService(EcoTurismoDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IEnumerable<string>> GetPermissionsByRoleIdAsync(Guid roleId)
    {
        var cacheKey = $"{CacheKeyPrefix}RoleId_{roleId}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<string>? cachedPermissions) && cachedPermissions != null)
            return cachedPermissions;

        var permissions = await _db.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(CacheMinutes));
        
        return permissions;
    }

    public async Task<IEnumerable<string>> GetPermissionsByRoleNameAsync(string roleName)
    {
        var normalizedName = roleName.ToUpperInvariant();
        var cacheKey = $"{CacheKeyPrefix}RoleName_{normalizedName}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<string>? cachedPermissions) && cachedPermissions != null)
            return cachedPermissions;

        var permissions = await _db.Roles
            .Where(r => r.NormalizedName == normalizedName && r.IsActive)
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(CacheMinutes));
        
        return permissions;
    }

    public async Task<bool> HasPermissionAsync(Guid roleId, string permissionName)
    {
        var permissions = await GetPermissionsByRoleIdAsync(roleId);
        return permissions.Contains(permissionName);
    }

    public async Task<bool> HasAnyPermissionAsync(Guid roleId, params string[] permissionNames)
    {
        var permissions = await GetPermissionsByRoleIdAsync(roleId);
        return permissionNames.Any(p => permissions.Contains(p));
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        var normalizedName = roleName.ToUpperInvariant();
        var cacheKey = $"{CacheKeyPrefix}Role_{normalizedName}";
        
        if (_cache.TryGetValue(cacheKey, out Role? cachedRole) && cachedRole != null)
            return cachedRole;

        var role = await _db.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedName && r.IsActive);

        if (role != null)
            _cache.Set(cacheKey, role, TimeSpan.FromMinutes(CacheMinutes));
        
        return role;
    }

    public async Task<Role?> GetRoleByIdAsync(Guid roleId)
    {
        var cacheKey = $"{CacheKeyPrefix}RoleById_{roleId}";
        
        if (_cache.TryGetValue(cacheKey, out Role? cachedRole) && cachedRole != null)
            return cachedRole;

        var role = await _db.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId && r.IsActive);

        if (role != null)
            _cache.Set(cacheKey, role, TimeSpan.FromMinutes(CacheMinutes));
        
        return role;
    }
}
