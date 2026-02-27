using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Infra.Data.Seeds;

public static class AuthorizationSeed
{
    public static async Task SeedAsync(EcoTurismoDbContext context)
    {
        // Verifica se já tem dados
        if (await context.Roles.AnyAsync())
            return;

        var now = DateTimeOffset.UtcNow;

        // 1. Criar Roles
        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            NormalizedName = "ADMIN",
            Description = "Administrador com acesso total ao sistema",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        var prefeituraRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Prefeitura",
            NormalizedName = "PREFEITURA",
            Description = "Gerencia banners, atrativos e visualiza relatórios",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        var balnearioRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Balneario",
            NormalizedName = "BALNEARIO",
            Description = "Gerencia quiosques e reservas",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        var publicoRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Publico",
            NormalizedName = "PUBLICO",
            Description = "Usuário público com permissões básicas",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.Roles.AddRangeAsync(adminRole, prefeituraRole, balnearioRole, publicoRole);
        await context.SaveChangesAsync();

        // 2. Criar Permissions
        var permissions = new List<Permission>
        {
            // Banners
            new() { Id = Guid.NewGuid(), Name = "banners:create", Resource = "banners", Action = "create", Description = "Criar banners", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "banners:read", Resource = "banners", Action = "read", Description = "Visualizar banners", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "banners:update", Resource = "banners", Action = "update", Description = "Atualizar banners", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "banners:delete", Resource = "banners", Action = "delete", Description = "Excluir banners", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "banners:reorder", Resource = "banners", Action = "reorder", Description = "Reordenar banners", CreatedAt = now },

            // Atrativos
            new() { Id = Guid.NewGuid(), Name = "atrativos:create", Resource = "atrativos", Action = "create", Description = "Criar atrativos", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "atrativos:read", Resource = "atrativos", Action = "read", Description = "Visualizar atrativos", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "atrativos:update", Resource = "atrativos", Action = "update", Description = "Atualizar atrativos", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "atrativos:delete", Resource = "atrativos", Action = "delete", Description = "Excluir atrativos", CreatedAt = now },

            // Quiosques
            new() { Id = Guid.NewGuid(), Name = "quiosques:create", Resource = "quiosques", Action = "create", Description = "Criar quiosques", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "quiosques:read", Resource = "quiosques", Action = "read", Description = "Visualizar quiosques", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "quiosques:update", Resource = "quiosques", Action = "update", Description = "Atualizar quiosques", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "quiosques:delete", Resource = "quiosques", Action = "delete", Description = "Excluir quiosques", CreatedAt = now },

            // Reservas
            new() { Id = Guid.NewGuid(), Name = "reservas:create", Resource = "reservas", Action = "create", Description = "Criar reservas", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "reservas:read", Resource = "reservas", Action = "read", Description = "Visualizar reservas", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "reservas:update", Resource = "reservas", Action = "update", Description = "Atualizar reservas", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "reservas:delete", Resource = "reservas", Action = "delete", Description = "Excluir reservas", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "reservas:validate", Resource = "reservas", Action = "validate", Description = "Validar tickets", CreatedAt = now },

            // Configurações
            new() { Id = Guid.NewGuid(), Name = "configuracoes:read", Resource = "configuracoes", Action = "read", Description = "Visualizar configurações", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "configuracoes:update", Resource = "configuracoes", Action = "update", Description = "Atualizar configurações", CreatedAt = now },

            // Perfis
            new() { Id = Guid.NewGuid(), Name = "profiles:create", Resource = "profiles", Action = "create", Description = "Criar perfis", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "profiles:read", Resource = "profiles", Action = "read", Description = "Visualizar perfis", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "profiles:update", Resource = "profiles", Action = "update", Description = "Atualizar perfis", CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "profiles:delete", Resource = "profiles", Action = "delete", Description = "Excluir perfis", CreatedAt = now },

            // Municípios
            new() { Id = Guid.NewGuid(), Name = "municipios:read", Resource = "municipios", Action = "read", Description = "Visualizar municípios", CreatedAt = now },
        };

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();

        // 3. Associar Permissions às Roles
        var rolePermissions = new List<RolePermission>();

        // Admin - todas as permissões
        foreach (var permission in permissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id,
                GrantedAt = now
            });
        }

        // Prefeitura
        var prefeituraPermissions = permissions.Where(p =>
            p.Resource == "banners" ||
            (p.Resource == "atrativos" && p.Action != "delete") ||
            (p.Resource == "quiosques" && p.Action == "read") ||
            (p.Resource == "reservas" && p.Action == "read") ||
            (p.Resource == "configuracoes" && p.Action == "read") ||
            p.Resource == "municipios"
        );

        foreach (var permission in prefeituraPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = prefeituraRole.Id,
                PermissionId = permission.Id,
                GrantedAt = now
            });
        }

        // Balneario
        var balnearioPermissions = permissions.Where(p =>
            p.Resource == "quiosques" ||
            (p.Resource == "reservas" && (p.Action == "create" || p.Action == "read" || p.Action == "update" || p.Action == "validate")) ||
            (p.Resource == "atrativos" && p.Action == "read") ||
            p.Resource == "municipios"
        );

        foreach (var permission in balnearioPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = balnearioRole.Id,
                PermissionId = permission.Id,
                GrantedAt = now
            });
        }

        // Publico
        var publicoPermissions = permissions.Where(p =>
            (p.Resource == "banners" && p.Action == "read") ||
            (p.Resource == "atrativos" && p.Action == "read") ||
            (p.Resource == "quiosques" && p.Action == "read") ||
            (p.Resource == "reservas" && (p.Action == "create" || p.Action == "read")) ||
            (p.Resource == "configuracoes" && p.Action == "read") ||
            p.Resource == "municipios"
        );

        foreach (var permission in publicoPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = publicoRole.Id,
                PermissionId = permission.Id,
                GrantedAt = now
            });
        }

        await context.RolePermissions.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();
    }
}
