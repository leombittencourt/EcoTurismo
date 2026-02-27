using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EcoTurismo.Infra.Data.Seeds;

public static class AuthorizationSeed
{
    public static async Task SeedAsync(EcoTurismoDbContext context)
    {        
        // Verifica se já tem dados
        if (await context.Roles.AnyAsync())
        {
            Console.WriteLine("ℹ️  Seed ignorado: Dados já existem no banco.");
            return;
        }

        Console.WriteLine("🌱 Iniciando seed de dados iniciais...");

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
        Console.WriteLine("   ✅ 26 Permissions criadas");

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

        // 4. Criar Município Base
        var municipioBase = new Municipio
        {
            Id = Guid.NewGuid(),
            Nome = "Rio Verde de Mato Grosso",
            Uf = "MS",
            Logo = null,
            CreatedAt = now
        };

        await context.Municipios.AddAsync(municipioBase);
        await context.SaveChangesAsync();
        Console.WriteLine($"   ✅ Município base criado: {municipioBase.Nome} - {municipioBase.Uf}");

        // 4.1. Criar Atrativo Inicial
        var atrativoInicial = new Atrativo
        {
            Id = Guid.NewGuid(),
            Nome = "Balneário Municipal",
            Tipo = "balneario",
            Descricao = "O Balneário Municipal de Rio Verde de Mato Grosso é um dos principais atrativos turísticos da região. Com águas cristalinas e infraestrutura completa, oferece lazer e diversão para toda a família. O local conta com piscinas naturais, áreas de camping, quiosques e playground.",
            MunicipioId = municipioBase.Id,
            Imagem = "https://via.placeholder.com/800x600?text=Balneario+Municipal",
            Status = "ativo",
            CapacidadeMaxima = 500,
            OcupacaoAtual = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        await context.Atrativos.AddAsync(atrativoInicial);
        await context.SaveChangesAsync();
        Console.WriteLine($"   ✅ Atrativo inicial criado: {atrativoInicial.Nome}");

        // 5. Criar Usuários Default (senha: admin123)
        Console.WriteLine("   🔐 Gerando hash de senha para usuários default...");
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        Console.WriteLine($"   ✅ Hash gerado: {passwordHash.Substring(0, 20)}...");

        var usuarios = new[]
        {
            new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "Administrador do Sistema",
                Email = "admin@ecoturismo.com.br",
                PasswordHash = passwordHash,
                RoleId = adminRole.Id,
                MunicipioId = municipioBase.Id,
                Telefone = "(67) 3000-0001",
                Ativo = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "Prefeitura Rio Verde",
                Email = "prefeitura@ecoturismo.com.br",
                PasswordHash = passwordHash,
                RoleId = prefeituraRole.Id,
                MunicipioId = municipioBase.Id,
                Telefone = "(67) 3000-0002",
                Ativo = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "Balneário Municipal",
                Email = "balneario@ecoturismo.com.br",
                PasswordHash = passwordHash,
                RoleId = balnearioRole.Id,
                MunicipioId = municipioBase.Id,
                AtrativoId = atrativoInicial.Id, // Vinculado ao Balneário Municipal
                Telefone = "(67) 3000-0003",
                Ativo = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "Usuário Público",
                Email = "publico@ecoturismo.com.br",
                PasswordHash = passwordHash,
                RoleId = publicoRole.Id,
                MunicipioId = municipioBase.Id,
                Telefone = "(67) 3000-0004",
                Ativo = true,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Usuarios.AddRangeAsync(usuarios);
        await context.SaveChangesAsync();
    }
}
