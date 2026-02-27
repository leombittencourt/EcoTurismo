using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Services;
using EcoTurismo.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using ApiLoginRequest = EcoTurismo.Api.Endpoints.Auth.LoginRequest;
using AppLoginRequest = EcoTurismo.Application.DTOs.LoginRequest;

namespace EcoTurismo.Tests.Integration;

public class LoginIntegrationTests
{
    [Fact]
    public async Task FluxoCompleto_LoginComSucesso()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        
        var role = TestDataBuilder.CreateRole("Admin");
        await context.Roles.AddAsync(role);

        var municipio = TestDataBuilder.CreateMunicipio();
        await context.Municipios.AddAsync(municipio);

        var usuario = new Domain.Entities.Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Admin Teste",
            Email = "admin@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
            RoleId = role.Id,
            MunicipioId = municipio.Id,
            Ativo = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var jwtSettingsMock = new Mock<IOptions<Application.Auth.JwtSettings>>();
        jwtSettingsMock.Setup(x => x.Value).Returns(new Application.Auth.JwtSettings
        {
            Key = "ChaveSecretaParaTestesComPeloMenos32Caracteres!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 60
        });

        var cache = new MemoryCache(new MemoryCacheOptions());
        var permissionService = new Application.Services.PermissionService(context, cache);
        var authService = new AuthService(context, jwtSettingsMock.Object, permissionService);
        var endpoint = new Api.Endpoints.Auth.LoginEndpoint(authService);

        var request = new ApiLoginRequest
        {
            Email = "admin@test.com",
            Password = "senha123"
        };

        // Act
        var loginRequest = new AppLoginRequest(request.Email, request.Password);
        var result = await authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Profile.Should().NotBeNull();
        result.Profile.Email.Should().Be("admin@test.com");
        result.Profile.Nome.Should().Be("Admin Teste");
        result.Profile.Role.Should().Be("Admin");
        result.Profile.MunicipioId.Should().Be(municipio.Id);
    }

    [Fact]
    public async Task FluxoCompleto_LoginComUsuarioInativo()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var usuario = new Domain.Entities.Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Usuario Inativo",
            Email = "inativo@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
            RoleId = role.Id,
            Ativo = false, // Usuário inativo
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var jwtSettingsMock = new Mock<IOptions<Application.Auth.JwtSettings>>();
        jwtSettingsMock.Setup(x => x.Value).Returns(new Application.Auth.JwtSettings
        {
            Key = "ChaveSecretaParaTestesComPeloMenos32Caracteres!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 60
        });

        var cache = new MemoryCache(new MemoryCacheOptions());
        var permissionService = new Application.Services.PermissionService(context, cache);
        var authService = new AuthService(context, jwtSettingsMock.Object, permissionService);

        var loginRequest = new Application.DTOs.LoginRequest("inativo@test.com", "senha123");

        // Act
        var result = await authService.LoginAsync(loginRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task FluxoCompleto_LoginComSenhaIncorreta()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var usuario = new Domain.Entities.Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Usuario Teste",
            Email = "user@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senhaCorreta"),
            RoleId = role.Id,
            Ativo = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var jwtSettingsMock = new Mock<IOptions<Application.Auth.JwtSettings>>();
        jwtSettingsMock.Setup(x => x.Value).Returns(new Application.Auth.JwtSettings
        {
            Key = "ChaveSecretaParaTestesComPeloMenos32Caracteres!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 60
        });

        var cache = new MemoryCache(new MemoryCacheOptions());
        var permissionService = new Application.Services.PermissionService(context, cache);
        var authService = new AuthService(context, jwtSettingsMock.Object, permissionService);

        var loginRequest = new AppLoginRequest("user@test.com", "senhaErrada");

        // Act
        var result = await authService.LoginAsync(loginRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task FluxoCompleto_LoginComEmailCaseInsensitive()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var usuario = new Domain.Entities.Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Usuario Teste",
            Email = "user@test.com", // lowercase
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
            RoleId = role.Id,
            Ativo = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var jwtSettingsMock = new Mock<IOptions<Application.Auth.JwtSettings>>();
        jwtSettingsMock.Setup(x => x.Value).Returns(new Application.Auth.JwtSettings
        {
            Key = "ChaveSecretaParaTestesComPeloMenos32Caracteres!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 60
        });

        var cache = new MemoryCache(new MemoryCacheOptions());
        var permissionService = new Application.Services.PermissionService(context, cache);
        var authService = new AuthService(context, jwtSettingsMock.Object, permissionService);

        var loginRequest = new Application.DTOs.LoginRequest("USER@TEST.COM", "senha123"); // UPPERCASE

        // Act
        var result = await authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Profile.Email.Should().Be("user@test.com");
    }

    [Fact]
    public async Task FluxoCompleto_LoginComPermissoes()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        
        var role = TestDataBuilder.CreateRole("Prefeitura");
        await context.Roles.AddAsync(role);

        var permission1 = new Domain.Entities.Permission
        {
            Id = Guid.NewGuid(),
            Name = "banners:create",
            Resource = "banners",
            Action = "create",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var permission2 = new Domain.Entities.Permission
        {
            Id = Guid.NewGuid(),
            Name = "banners:read",
            Resource = "banners",
            Action = "read",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Permissions.AddRangeAsync(permission1, permission2);

        var rolePermission1 = new Domain.Entities.RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission1.Id,
            GrantedAt = DateTimeOffset.UtcNow
        };

        var rolePermission2 = new Domain.Entities.RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission2.Id,
            GrantedAt = DateTimeOffset.UtcNow
        };

        await context.RolePermissions.AddRangeAsync(rolePermission1, rolePermission2);

        var usuario = new Domain.Entities.Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Prefeitura Teste",
            Email = "prefeitura@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
            RoleId = role.Id,
            Ativo = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var jwtSettingsMock = new Mock<IOptions<Application.Auth.JwtSettings>>();
        jwtSettingsMock.Setup(x => x.Value).Returns(new Application.Auth.JwtSettings
        {
            Key = "ChaveSecretaParaTestesComPeloMenos32Caracteres!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 60
        });

        var cache = new MemoryCache(new MemoryCacheOptions());
        var permissionService = new Application.Services.PermissionService(context, cache);
        var authService = new AuthService(context, jwtSettingsMock.Object, permissionService);

        var loginRequest = new AppLoginRequest("prefeitura@test.com", "senha123");

        // Act
        var result = await authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Profile.Role.Should().Be("Prefeitura");
        
        // Verificar que as permissões foram carregadas
        var permissions = await permissionService.GetPermissionsByRoleIdAsync(role.Id);
        permissions.Should().HaveCount(2);
        permissions.Should().Contain("banners:create");
        permissions.Should().Contain("banners:read");
    }
}
