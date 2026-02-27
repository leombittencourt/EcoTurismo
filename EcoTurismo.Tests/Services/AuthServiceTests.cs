using EcoTurismo.Application.Auth;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Application.Services;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace EcoTurismo.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly Mock<IPermissionService> _permissionServiceMock;

    public AuthServiceTests()
    {
        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
        _jwtSettingsMock.Setup(x => x.Value).Returns(new JwtSettings
        {
            Key = "ChaveSecretaParaTestesComPeloMenos32Caracteres!",
            Issuer = "EcoTurismoTestApi",
            Audience = "EcoTurismoTestApp",
            ExpirationMinutes = 60
        });

        _permissionServiceMock = new Mock<IPermissionService>();
    }

    [Fact]
    public async Task LoginAsync_DeveRetornarTokenQuandoCredenciaisValidas()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole("Admin");
        await context.Roles.AddAsync(role);

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Test User",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
            RoleId = role.Id,
            Ativo = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        _permissionServiceMock
            .Setup(x => x.GetPermissionsByRoleIdAsync(role.Id))
            .ReturnsAsync(new List<string> { "banners:read", "atrativos:read" });

        var service = new AuthService(context, _jwtSettingsMock.Object, _permissionServiceMock.Object);
        var request = new LoginRequest("test@example.com", "senha123");

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Profile.Should().NotBeNull();
        result.Profile.Email.Should().Be("test@example.com");
        result.Profile.Nome.Should().Be("Test User");
        result.Profile.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task LoginAsync_DeveRetornarNullQuandoEmailNaoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var service = new AuthService(context, _jwtSettingsMock.Object, _permissionServiceMock.Object);
        var request = new LoginRequest("naoexiste@example.com", "senha123");

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_DeveRetornarNullQuandoSenhaIncorreta()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Test User",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senhaCorreta"),
            RoleId = role.Id,
            Ativo = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var service = new AuthService(context, _jwtSettingsMock.Object, _permissionServiceMock.Object);
        var request = new LoginRequest("test@example.com", "senhaErrada");

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_DeveIncluirPermissoesNoToken()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole("Prefeitura");
        await context.Roles.AddAsync(role);

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Prefeitura User",
            Email = "prefeitura@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
            RoleId = role.Id,
            Ativo = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var expectedPermissions = new List<string>
        {
            "banners:create",
            "banners:read",
            "atrativos:read"
        };

        _permissionServiceMock
            .Setup(x => x.GetPermissionsByRoleIdAsync(role.Id))
            .ReturnsAsync(expectedPermissions);

        var service = new AuthService(context, _jwtSettingsMock.Object, _permissionServiceMock.Object);
        var request = new LoginRequest("prefeitura@example.com", "senha123");

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();

        // Verifica se o método de buscar permissões foi chamado
        _permissionServiceMock.Verify(
            x => x.GetPermissionsByRoleIdAsync(role.Id),
            Times.Once);
    }
}
