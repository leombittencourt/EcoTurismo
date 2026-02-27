using EcoTurismo.Api.Endpoints.Auth;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace EcoTurismo.Tests.Endpoints;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator;

    public LoginValidatorTests()
    {
        _validator = new LoginValidator();
    }

    [Fact]
    public void Validate_ComEmailEPasswordValidos_NaoDeveTerErros()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@ecoturismo.com.br",
            Password = "admin123"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ComEmailVazio_DeveTerErro()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "",
            Password = "admin123"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email é obrigatório");
    }

    [Fact]
    public void Validate_ComEmailNull_DeveTerErro()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = null!,
            Password = "admin123"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ComEmailInvalido_DeveTerErro()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "emailinvalido",
            Password = "admin123"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email inválido");
    }

    [Theory]
    [InlineData("teste@")]
    [InlineData("@example.com")]
    [InlineData("teste@.com")]
    [InlineData("teste.example.com")]
    public void Validate_ComEmailMalFormatado_DeveTerErro(string email)
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = email,
            Password = "admin123"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ComPasswordVazio_DeveTerErro()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@ecoturismo.com.br",
            Password = ""
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Senha é obrigatória");
    }

    [Fact]
    public void Validate_ComPasswordNull_DeveTerErro()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@ecoturismo.com.br",
            Password = null!
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ComPasswordMuitoCurto_DeveTerErro()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@ecoturismo.com.br",
            Password = "ab" // Menos de 3 caracteres
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Senha deve ter no mínimo 3 caracteres");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("1234")]
    [InlineData("password")]
    [InlineData("admin123")]
    [InlineData("SenhaForte@123")]
    public void Validate_ComPasswordValido_NaoDeveTerErro(string password)
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@ecoturismo.com.br",
            Password = password
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@example.com")]
    [InlineData("user+tag@example.com")]
    [InlineData("user@subdomain.example.com")]
    [InlineData("admin@ecoturismo.com.br")]
    public void Validate_ComEmailsValidos_NaoDeveTerErro(string email)
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = email,
            Password = "admin123"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ComEmailEPasswordVazios_DeveTerErrosEmAmbos()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "",
            Password = ""
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ComEspacosNoEmail_DeveTerErro()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin @ecoturismo.com.br",
            Password = "admin123"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ComPasswordApenasEspacos_DeveTerErro()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@ecoturismo.com.br",
            Password = "   "
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        // Deve ter erro, pois depois de trim ficará vazio
        result.IsValid.Should().BeFalse();
    }
}
