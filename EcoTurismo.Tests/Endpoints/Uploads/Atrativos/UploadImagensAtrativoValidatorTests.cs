using EcoTurismo.Api.Endpoints.Uploads.Atrativos;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;

namespace EcoTurismo.Tests.Endpoints.Uploads.Atrativos;

public class UploadImagensAtrativoValidatorTests
{
    private readonly UploadImagensAtrativoValidator _validator;

    public UploadImagensAtrativoValidatorTests()
    {
        _validator = new UploadImagensAtrativoValidator();
    }

    [Fact]
    public void Validate_AtrativoIdVazio_DeveRetornarErro()
    {
        // Arrange
        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = Guid.Empty,
            Imagens = new List<IFormFile> { CreateFakeImageFile("test.jpg", 1024) }
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
        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<IFormFile>()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Imagens)
            .WithErrorMessage("É necessário enviar pelo menos uma imagem.");
    }

    [Fact]
    public void Validate_MaisDe10Imagens_DeveRetornarErro()
    {
        // Arrange
        var files = Enumerable.Range(1, 11)
            .Select(i => CreateFakeImageFile($"foto{i}.jpg", 1024))
            .ToArray();

        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = files.ToList()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Imagens)
            .WithErrorMessage("Máximo de 10 imagens por upload.");
    }

    [Fact]
    public void Validate_ImagemMuitoGrande_DeveRetornarErro()
    {
        // Arrange
        var file = CreateFakeImageFile("grande.jpg", 6 * 1024 * 1024); // 6MB

        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<IFormFile> { file }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Imagens)
            .WithErrorMessage("Cada imagem não pode ter mais de 5MB.");
    }

    [Fact]
    public void Validate_FormatoInvalido_DeveRetornarErro()
    {
        // Arrange
        var file = CreateFakeFile("documento.pdf", "application/pdf", 1024);

        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<IFormFile> { file }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Imagens)
            .WithErrorMessage("Arquivo deve ser uma imagem válida (jpg, jpeg, png, gif, webp).");
    }

    [Theory]
    [InlineData("foto.jpg")]
    [InlineData("foto.jpeg")]
    [InlineData("foto.png")]
    [InlineData("foto.gif")]
    [InlineData("foto.webp")]
    public void Validate_FormatosValidos_NãoDeveRetornarErro(string fileName)
    {
        // Arrange
        var file = CreateFakeImageFile(fileName, 1024);

        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<IFormFile> { file }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Imagens);
    }

    [Fact]
    public void Validate_DescricoesComTamanhoErrado_DeveRetornarErro()
    {
        // Arrange
        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<IFormFile>
            {
                CreateFakeImageFile("foto1.jpg", 1024),
                CreateFakeImageFile("foto2.jpg", 1024)
            },
            Descricoes = new[] { "Descrição 1" } // Falta uma descrição
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Descricoes)
            .WithErrorMessage("Quantidade de descrições deve ser igual à quantidade de imagens.");
    }

    [Fact]
    public void Validate_OrdensComTamanhoErrado_DeveRetornarErro()
    {
        // Arrange
        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<IFormFile>
            {
                CreateFakeImageFile("foto1.jpg", 1024),
                CreateFakeImageFile("foto2.jpg", 1024)
            },
            Ordens = new[] { 1, 2, 3 } // Uma ordem a mais
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Ordens)
            .WithErrorMessage("Quantidade de ordens deve ser igual à quantidade de imagens.");
    }

    [Fact]
    public void Validate_RequestValido_NaoDeveRetornarErros()
    {
        // Arrange
        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<IFormFile>
            {
                CreateFakeImageFile("foto1.jpg", 1024),
                CreateFakeImageFile("foto2.png", 2048)
            },
            Descricoes = new[] { "Foto 1", "Foto 2" },
            Ordens = new[] { 1, 2 }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static IFormFile CreateFakeImageFile(string fileName, int size)
    {
        var content = new byte[size];
        Array.Fill(content, (byte)0xFF);

        var stream = new MemoryStream(content);
        var file = new FormFile(stream, 0, size, "Imagens", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = fileName.EndsWith(".png") ? "image/png" : "image/jpeg"
        };

        return file;
    }

    private static IFormFile CreateFakeFile(string fileName, string contentType, int size)
    {
        var content = new byte[size];
        var stream = new MemoryStream(content);
        var file = new FormFile(stream, 0, size, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

        return file;
    }
}
