using EcoTurismo.Api.Endpoints.Uploads;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;

namespace EcoTurismo.Tests.Validators;

public class UploadBannerValidatorTests
{
    private readonly UploadBannerValidator _validator;

    public UploadBannerValidatorTests()
    {
        _validator = new UploadBannerValidator();
    }

    [Fact]
    public void Validate_DeveRetornarErroQuandoImagemNull()
    {
        // Arrange
        var request = new UploadBannerRequest
        {
            Imagem = null!,
            Titulo = "Banner Teste"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Imagem)
            .WithErrorMessage("A imagem é obrigatória.");
    }

    [Fact]
    public void Validate_DeveRetornarErroParaTituloVazio()
    {
        // Arrange
        var imagemMock = CreateValidImageMock();
        var request = new UploadBannerRequest
        {
            Imagem = imagemMock.Object,
            Titulo = ""
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorMessage("O título é obrigatório.");
    }

    [Fact]
    public void Validate_DeveRetornarErroParaTituloMuitoLongo()
    {
        // Arrange
        var imagemMock = CreateValidImageMock();
        var request = new UploadBannerRequest
        {
            Imagem = imagemMock.Object,
            Titulo = new string('A', 201) // 201 caracteres
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorMessage("O título não pode ter mais de 200 caracteres.");
    }

    [Fact]
    public void Validate_DeveRetornarErroParaFormatoInvalido()
    {
        // Arrange
        var imagemMock = new Mock<IFormFile>();
        imagemMock.Setup(x => x.FileName).Returns("document.pdf");
        imagemMock.Setup(x => x.Length).Returns(1000);

        var request = new UploadBannerRequest
        {
            Imagem = imagemMock.Object,
            Titulo = "Banner Teste"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Imagem)
            .WithErrorMessage("O arquivo deve ser uma imagem válida (jpg, jpeg, png, gif, webp).");
    }

    [Fact]
    public void Validate_DeveRetornarErroParaArquivoMuitoGrande()
    {
        // Arrange
        var imagemMock = new Mock<IFormFile>();
        imagemMock.Setup(x => x.FileName).Returns("image.png");
        imagemMock.Setup(x => x.Length).Returns(6 * 1024 * 1024); // 6MB

        var request = new UploadBannerRequest
        {
            Imagem = imagemMock.Object,
            Titulo = "Banner Teste"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Imagem)
            .WithErrorMessage("A imagem não pode ter mais de 5MB.");
    }

    [Fact]
    public void Validate_DevePassarParaRequestValido()
    {
        // Arrange
        var imagemMock = CreateValidImageMock();
        var request = new UploadBannerRequest
        {
            Imagem = imagemMock.Object,
            Titulo = "Banner Teste",
            Subtitulo = "Subtítulo válido",
            Link = "https://example.com",
            Ordem = 1,
            Ativo = true
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(".jpg")]
    [InlineData(".jpeg")]
    [InlineData(".png")]
    [InlineData(".gif")]
    [InlineData(".webp")]
    [InlineData(".JPG")]
    [InlineData(".PNG")]
    public void Validate_DeveAceitarFormatosValidos(string extension)
    {
        // Arrange
        var imagemMock = new Mock<IFormFile>();
        imagemMock.Setup(x => x.FileName).Returns($"image{extension}");
        imagemMock.Setup(x => x.Length).Returns(1024 * 1024); // 1MB

        var request = new UploadBannerRequest
        {
            Imagem = imagemMock.Object,
            Titulo = "Banner Teste"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Imagem);
    }

    [Fact]
    public void Validate_DeveRetornarErroParaSubtituloMuitoLongo()
    {
        // Arrange
        var imagemMock = CreateValidImageMock();
        var request = new UploadBannerRequest
        {
            Imagem = imagemMock.Object,
            Titulo = "Banner Teste",
            Subtitulo = new string('B', 501) // 501 caracteres
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Subtitulo)
            .WithErrorMessage("O subtítulo não pode ter mais de 500 caracteres.");
    }

    [Fact]
    public void Validate_DeveRetornarErroParaOrdemNegativa()
    {
        // Arrange
        var imagemMock = CreateValidImageMock();
        var request = new UploadBannerRequest
        {
            Imagem = imagemMock.Object,
            Titulo = "Banner Teste",
            Ordem = -1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ordem)
            .WithErrorMessage("A ordem deve ser um número positivo.");
    }

    private static Mock<IFormFile> CreateValidImageMock()
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(x => x.FileName).Returns("test.png");
        mock.Setup(x => x.Length).Returns(1024 * 1024); // 1MB
        return mock;
    }
}
