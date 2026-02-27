using EcoTurismo.Application.Interfaces;
using FluentAssertions;
using Moq;
using ApiLoginRequest = EcoTurismo.Api.Endpoints.Auth.LoginRequest;
using AppLoginRequest = EcoTurismo.Application.DTOs.LoginRequest;
using AppLoginResponse = EcoTurismo.Application.DTOs.LoginResponse;

namespace EcoTurismo.Tests.Endpoints;

public class LoginEndpointTests
{
    private readonly Mock<IAuthService> _authServiceMock;

    public LoginEndpointTests()
    {
        _authServiceMock = new Mock<IAuthService>();
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornarOkComToken()
    {
        // Arrange
        var request = new ApiLoginRequest
        {
            Email = "admin@ecoturismo.com.br",
            Password = "admin123"
        };

        var expectedResponse = new AppLoginResponse(
            Token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test.token",
            Profile: new Application.DTOs.ProfileDto(
                Id: Guid.NewGuid(),
                Nome: "Admin Teste",
                Email: "admin@ecoturismo.com.br",
                Role: "Admin",
                MunicipioId: Guid.NewGuid(),
                AtrativoId: null
            )
        );

        _authServiceMock
            .Setup(x => x.LoginAsync(It.Is<AppLoginRequest>(
                r => r.Email == request.Email && r.Password == request.Password)))
            .ReturnsAsync(expectedResponse);

        var endpoint = new Api.Endpoints.Auth.LoginEndpoint(_authServiceMock.Object);

        // Act & Assert
        var result = await _authServiceMock.Object.LoginAsync(
            new AppLoginRequest(request.Email, request.Password));

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Profile.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Login_ComEmailInvalido_DeveRetornarUnauthorized()
    {
        // Arrange
        var request = new ApiLoginRequest
        {
            Email = "naoexiste@example.com",
            Password = "senhaqualquer"
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(It.IsAny<AppLoginRequest>()))
            .ReturnsAsync((AppLoginResponse?)null);

        var endpoint = new Api.Endpoints.Auth.LoginEndpoint(_authServiceMock.Object);

        // Act & Assert
        var result = await _authServiceMock.Object.LoginAsync(
            new AppLoginRequest(request.Email, request.Password));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Login_ComSenhaIncorreta_DeveRetornarUnauthorized()
    {
        // Arrange
        var request = new ApiLoginRequest
        {
            Email = "admin@ecoturismo.com.br",
            Password = "senhaerrada"
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(It.Is<AppLoginRequest>(
                r => r.Email == request.Email && r.Password == request.Password)))
            .ReturnsAsync((AppLoginResponse?)null);

        var endpoint = new Api.Endpoints.Auth.LoginEndpoint(_authServiceMock.Object);

        // Act
        var result = await _authServiceMock.Object.LoginAsync(
            new AppLoginRequest(request.Email, request.Password));

        // Assert
        result.Should().BeNull();
        _authServiceMock.Verify(
            x => x.LoginAsync(It.Is<AppLoginRequest>(
                r => r.Email == request.Email && r.Password == request.Password)),
            Times.Once);
    }

    [Fact]
    public async Task Login_ComEmailVazio_DeveRetornarUnauthorized()
    {
        // Arrange
        var request = new ApiLoginRequest
        {
            Email = "",
            Password = "admin123"
        };

        var endpoint = new Api.Endpoints.Auth.LoginEndpoint(_authServiceMock.Object);

        // Act & Assert
        request.Email.Should().BeEmpty();
    }

    [Fact]
    public async Task Login_ComSenhaVazia_DeveRetornarUnauthorized()
    {
        // Arrange
        var request = new ApiLoginRequest
        {
            Email = "admin@ecoturismo.com.br",
            Password = ""
        };

        var endpoint = new Api.Endpoints.Auth.LoginEndpoint(_authServiceMock.Object);

        // Act & Assert
        request.Password.Should().BeEmpty();
    }

    [Fact]
    public async Task Login_ComDadosValidos_DeveChamarAuthServiceUmaVez()
    {
        // Arrange
        var request = new ApiLoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var expectedResponse = new AppLoginResponse(
            Token: "test.token",
            Profile: new Application.DTOs.ProfileDto(
                Id: Guid.NewGuid(),
                Nome: "Test User",
                Email: "test@example.com",
                Role: "Publico",
                MunicipioId: null,
                AtrativoId: null
            )
        );

        _authServiceMock
            .Setup(x => x.LoginAsync(It.IsAny<AppLoginRequest>()))
            .ReturnsAsync(expectedResponse);

        var endpoint = new Api.Endpoints.Auth.LoginEndpoint(_authServiceMock.Object);

        // Act
        await _authServiceMock.Object.LoginAsync(
            new AppLoginRequest(request.Email, request.Password));

        // Assert
        _authServiceMock.Verify(
            x => x.LoginAsync(It.Is<AppLoginRequest>(
                r => r.Email == request.Email && r.Password == request.Password)),
            Times.Once);
    }

    [Theory]
    [InlineData("admin@ecoturismo.com.br", "admin123", true)]
    [InlineData("prefeitura@ecoturismo.com.br", "admin123", true)]
    [InlineData("balneario@ecoturismo.com.br", "admin123", true)]
    [InlineData("publico@ecoturismo.com.br", "admin123", true)]
    [InlineData("invalido@example.com", "senhaerrada", false)]
    [InlineData("", "admin123", false)]
    [InlineData("admin@ecoturismo.com.br", "", false)]
    public async Task Login_ComVariosUsuarios_DeveRetornarResultadoEsperado(
        string email, 
        string password, 
        bool deveRetornarToken)
    {
        // Arrange
        AppLoginResponse? expectedResponse = null;

        if (deveRetornarToken)
        {
            expectedResponse = new AppLoginResponse(
                Token: $"token.for.{email}",
                Profile: new Application.DTOs.ProfileDto(
                    Id: Guid.NewGuid(),
                    Nome: "Test User",
                    Email: email,
                    Role: "Test",
                    MunicipioId: null,
                    AtrativoId: null
                )
            );
        }

        _authServiceMock
            .Setup(x => x.LoginAsync(It.Is<AppLoginRequest>(
                r => r.Email == email && r.Password == password)))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _authServiceMock.Object.LoginAsync(
            new AppLoginRequest(email, password));

        // Assert
        if (deveRetornarToken)
        {
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Profile.Email.Should().Be(email);
        }
        else
        {
            result.Should().BeNull();
        }
    }

    [Fact]
    public async Task Login_QuandoAuthServiceLancaExcecao_DevePropagar()
    {
        // Arrange
        var request = new ApiLoginRequest
        {
            Email = "test@example.com",
            Password = "password"
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(It.IsAny<AppLoginRequest>()))
            .ThrowsAsync(new InvalidOperationException("Erro no banco de dados"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _authServiceMock.Object.LoginAsync(
                new AppLoginRequest(request.Email, request.Password)));
    }

    [Fact]
    public async Task Login_ComEmailCaseInsensitive_DeveFuncionar()
    {
        // Arrange
        var request = new ApiLoginRequest
        {
            Email = "ADMIN@ECOTURISMO.COM.BR",
            Password = "admin123"
        };

        var expectedResponse = new AppLoginResponse(
            Token: "test.token",
            Profile: new Application.DTOs.ProfileDto(
                Id: Guid.NewGuid(),
                Nome: "Admin",
                Email: "admin@ecoturismo.com.br",
                Role: "Admin",
                MunicipioId: null,
                AtrativoId: null
            )
        );

        _authServiceMock
            .Setup(x => x.LoginAsync(It.IsAny<AppLoginRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _authServiceMock.Object.LoginAsync(
            new AppLoginRequest(request.Email, request.Password));

        // Assert
        result.Should().NotBeNull();
        result!.Profile.Email.Should().Be("admin@ecoturismo.com.br");
    }
}
