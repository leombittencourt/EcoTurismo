using EcoTurismo.Api.Endpoints.Uploads.Atrativos;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace EcoTurismo.Tests.Endpoints.Uploads.Atrativos;

public class ReordenarImagensValidatorTests
{
    private readonly ReordenarImagensValidator _validator;

    public ReordenarImagensValidatorTests()
    {
        _validator = new ReordenarImagensValidator();
    }

    [Fact]
    public void Validate_AtrativoIdVazio_DeveRetornarErro()
    {
        // Arrange
        var request = new ReordenarImagensRequest
        {
            AtrativoId = Guid.Empty,
            Imagens = new List<ImagemOrdemDto>
            {
                new("id1", 1)
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.AtrativoId);
    }

    [Fact]
    public void Validate_SemImagens_DeveRetornarErro()
    {
        // Arrange
        var request = new ReordenarImagensRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<ImagemOrdemDto>()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Imagens)
            .WithErrorMessage("É necessário enviar pelo menos uma imagem para reordenar.");
    }

    [Fact]
    public void Validate_ImagemSemId_DeveRetornarErro()
    {
        // Arrange
        var request = new ReordenarImagensRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<ImagemOrdemDto>
            {
                new("", 1)
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Imagens[0]")
            .WithErrorMessage("Cada imagem deve ter um ID válido.");
    }

    [Fact]
    public void Validate_OrdemZeroOuNegativa_DeveRetornarErro()
    {
        // Arrange
        var request = new ReordenarImagensRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<ImagemOrdemDto>
            {
                new("id1", 0),
                new("id2", -1)
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Imagens[0]")
            .WithErrorMessage("A ordem deve ser um número positivo.");
        result.ShouldHaveValidationErrorFor("Imagens[1]")
            .WithErrorMessage("A ordem deve ser um número positivo.");
    }

    [Fact]
    public void Validate_RequestValido_NaoDeveRetornarErros()
    {
        // Arrange
        var request = new ReordenarImagensRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<ImagemOrdemDto>
            {
                new("id1", 1),
                new("id2", 2),
                new("id3", 3)
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_OrdensNaoSequenciais_NaoDeveRetornarErro()
    {
        // Arrange
        var request = new ReordenarImagensRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<ImagemOrdemDto>
            {
                new("id1", 10),
                new("id2", 5),
                new("id3", 1)
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
