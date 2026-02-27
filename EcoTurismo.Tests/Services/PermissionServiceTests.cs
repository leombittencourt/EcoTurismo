using EcoTurismo.Application.Services;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace EcoTurismo.Tests.Services;

public class PermissionServiceTests
{
    private readonly IMemoryCache _cache;

    public PermissionServiceTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public async Task GetPermissionsByRoleIdAsync_DeveRetornarPermissoesCorretas()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();

        var role = TestDataBuilder.CreateRole("Admin");
        await context.Roles.AddAsync(role);

        var permission1 = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "banners:create",
            Resource = "banners",
            Action = "create",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var permission2 = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "banners:read",
            Resource = "banners",
            Action = "read",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Permissions.AddRangeAsync(permission1, permission2);

        var rolePermission1 = new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission1.Id,
            GrantedAt = DateTimeOffset.UtcNow
        };

        var rolePermission2 = new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission2.Id,
            GrantedAt = DateTimeOffset.UtcNow
        };

        await context.RolePermissions.AddRangeAsync(rolePermission1, rolePermission2);
        await context.SaveChangesAsync();

        var service = new PermissionService(context, _cache);

        // Act
        var result = await service.GetPermissionsByRoleIdAsync(role.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("banners:create");
        result.Should().Contain("banners:read");
    }

    [Fact]
    public async Task GetPermissionsByRoleNameAsync_DeveRetornarPermissoesCorretas()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();

        var role = TestDataBuilder.CreateRole("Prefeitura");
        await context.Roles.AddAsync(role);

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "atrativos:create",
            Resource = "atrativos",
            Action = "create",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Permissions.AddAsync(permission);

        var rolePermission = new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id,
            GrantedAt = DateTimeOffset.UtcNow
        };

        await context.RolePermissions.AddAsync(rolePermission);
        await context.SaveChangesAsync();

        var service = new PermissionService(context, _cache);

        // Act
        var result = await service.GetPermissionsByRoleNameAsync("Prefeitura");

        // Assert
        result.Should().ContainSingle();
        result.Should().Contain("atrativos:create");
    }

    [Fact]
    public async Task HasPermissionAsync_DeveRetornarTrueQuandoTemPermissao()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();

        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "banners:delete",
            Resource = "banners",
            Action = "delete",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Permissions.AddAsync(permission);

        var rolePermission = new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id,
            GrantedAt = DateTimeOffset.UtcNow
        };

        await context.RolePermissions.AddAsync(rolePermission);
        await context.SaveChangesAsync();

        var service = new PermissionService(context, _cache);

        // Act
        var result = await service.HasPermissionAsync(role.Id, "banners:delete");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasPermissionAsync_DeveRetornarFalseQuandoNaoTemPermissao()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();

        var service = new PermissionService(context, _cache);

        // Act
        var result = await service.HasPermissionAsync(role.Id, "banners:delete");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasAnyPermissionAsync_DeveRetornarTrueQuandoTemQualquerUma()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();

        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "banners:read",
            Resource = "banners",
            Action = "read",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Permissions.AddAsync(permission);

        var rolePermission = new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id,
            GrantedAt = DateTimeOffset.UtcNow
        };

        await context.RolePermissions.AddAsync(rolePermission);
        await context.SaveChangesAsync();

        var service = new PermissionService(context, _cache);

        // Act
        var result = await service.HasAnyPermissionAsync(
            role.Id,
            "banners:create",
            "banners:read", // Esta o usuário tem
            "banners:delete");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetRoleByNameAsync_DeveRetornarRoleQuandoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole("Balneario");
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();

        var service = new PermissionService(context, _cache);

        // Act
        var result = await service.GetRoleByNameAsync("Balneario");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Balneario");
        result.NormalizedName.Should().Be("BALNEARIO");
    }

    [Fact]
    public async Task GetRoleByIdAsync_DeveRetornarRoleQuandoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole("Publico");
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();

        var service = new PermissionService(context, _cache);

        // Act
        var result = await service.GetRoleByIdAsync(role.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(role.Id);
        result.Name.Should().Be("Publico");
    }

    [Fact]
    public async Task GetPermissionsByRoleIdAsync_DeveFazerCacheDasPermissoes()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "test:permission",
            Resource = "test",
            Action = "permission",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Permissions.AddAsync(permission);

        var rolePermission = new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id,
            GrantedAt = DateTimeOffset.UtcNow
        };

        await context.RolePermissions.AddAsync(rolePermission);
        await context.SaveChangesAsync();

        var service = new PermissionService(context, _cache);

        // Act - Primeira chamada (popula cache)
        var result1 = await service.GetPermissionsByRoleIdAsync(role.Id);

        // Act - Segunda chamada (deve vir do cache)
        var result2 = await service.GetPermissionsByRoleIdAsync(role.Id);

        // Assert
        result1.Should().BeEquivalentTo(result2);
        result1.Should().ContainSingle();
        result1.Should().Contain("test:permission");
    }
}
