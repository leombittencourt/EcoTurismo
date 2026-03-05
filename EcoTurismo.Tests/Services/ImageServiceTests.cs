using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Application.Services;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace EcoTurismo.Tests.Services;

public class ImageServiceTests
{
    private readonly Mock<IStorageProvider> _storageProviderMock;
    private readonly Mock<ILogger<ImageService>> _loggerMock;

    public ImageServiceTests()
    {
        _storageProviderMock = new Mock<IStorageProvider>();
        _loggerMock = new Mock<ILogger<ImageService>>();

        _storageProviderMock.Setup(x => x.ProviderName).Returns("base64");
    }

    [Fact]
    public async Task ValidarImagemAsync_DeveRetornarErroQuandoArquivoVazio()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var service = new ImageService(context, _loggerMock.Object, _storageProviderMock.Object);
        
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(x => x.Length).Returns(0);

        // Act
        var result = await service.ValidarImagemAsync(fileMock.Object);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("vazio");
    }

    [Fact]
    public async Task ValidarImagemAsync_DeveRetornarErroParaFormatoInvalido()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var service = new ImageService(context, _loggerMock.Object, _storageProviderMock.Object);
        
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(x => x.Length).Returns(1000);
        fileMock.Setup(x => x.FileName).Returns("document.pdf");

        // Act
        var result = await service.ValidarImagemAsync(fileMock.Object);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("não suportado");
    }

    [Fact]
    public async Task ValidarImagemAsync_DeveRetornarErroParaArquivoMuitoGrande()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var service = new ImageService(context, _loggerMock.Object, _storageProviderMock.Object);
        
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(x => x.Length).Returns(6 * 1024 * 1024); // 6MB
        fileMock.Setup(x => x.FileName).Returns("image.png");

        // Act
        var result = await service.ValidarImagemAsync(fileMock.Object);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("muito grande");
    }

    [Fact]
    public async Task ValidarImagemAsync_DeveRetornarSucessoParaImagemValida()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var service = new ImageService(context, _loggerMock.Object, _storageProviderMock.Object);
        
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(x => x.Length).Returns(1024 * 1024); // 1MB
        fileMock.Setup(x => x.FileName).Returns("image.jpg");

        // Act
        var result = await service.ValidarImagemAsync(fileMock.Object);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task SalvarImagemAsync_DeveCriarImagemComSucesso()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        
        // Criar uma imagem PNG 1x1 válida
        var pngBytes = CreateValidPngBytes();
        
        _storageProviderMock
            .Setup(x => x.SaveImageAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("data:image/png;base64,iVBORw0KGgo=");

        var service = new ImageService(context, _loggerMock.Object, _storageProviderMock.Object);
        
        var request = new ImagemUploadRequest(
            EntidadeTipo: "Banner",
            EntidadeId: Guid.NewGuid(),
            Categoria: "principal",
            ImagemBytes: pngBytes,
            NomeArquivo: "test.png",
            TipoMime: "image/png",
            Ordem: 0
        );

        // Act
        var result = await service.SalvarImagemAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EntidadeTipo.Should().Be("Banner");
        result.Data.Categoria.Should().Be("principal");
        result.Data.StorageProvider.Should().Be("base64");
        
        var imagemSalva = await context.Imagens.FindAsync(result.Data.Id);
        imagemSalva.Should().NotBeNull();
    }

    [Fact]
    public async Task ListarImagensAsync_DeveRetornarImagensDaEntidade()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var entidadeId = Guid.NewGuid();

        var imagem1 = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Banner",
            EntidadeId = entidadeId,
            Categoria = "principal",
            ImagemUrl = "data:image/png;base64,test1",
            Ordem = 0,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var imagem2 = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Banner",
            EntidadeId = entidadeId,
            Categoria = "galeria",
            ImagemUrl = "data:image/png;base64,test2",
            Ordem = 1,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var imagemOutraEntidade = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Atrativo",
            EntidadeId = Guid.NewGuid(),
            Categoria = "principal",
            ImagemUrl = "data:image/png;base64,test3",
            Ordem = 0,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Imagens.AddRangeAsync(imagem1, imagem2, imagemOutraEntidade);
        await context.SaveChangesAsync();

        var service = new ImageService(context, _loggerMock.Object, _storageProviderMock.Object);

        // Act
        var result = await service.ListarImagensAsync("Banner", entidadeId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(i => i.EntidadeId == entidadeId);
    }

    [Fact]
    public async Task ListarImagensPorCategoriaAsync_DeveRetornarApenasDaCategoria()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var entidadeId = Guid.NewGuid();

        var imagemPrincipal = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Banner",
            EntidadeId = entidadeId,
            Categoria = "principal",
            ImagemUrl = "data:image/png;base64,test1",
            Ordem = 0,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var imagemGaleria = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Banner",
            EntidadeId = entidadeId,
            Categoria = "galeria",
            ImagemUrl = "data:image/png;base64,test2",
            Ordem = 1,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Imagens.AddRangeAsync(imagemPrincipal, imagemGaleria);
        await context.SaveChangesAsync();

        var service = new ImageService(context, _loggerMock.Object, _storageProviderMock.Object);

        // Act
        var result = await service.ListarImagensPorCategoriaAsync("Banner", entidadeId, "principal");

        // Assert
        result.Should().HaveCount(1);
        result.First().Categoria.Should().Be("principal");
    }

    [Fact]
    public async Task RemoverImagemAsync_DeveRemoverComSucesso()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var imagem = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Banner",
            EntidadeId = Guid.NewGuid(),
            Categoria = "principal",
            ImagemUrl = "data:image/png;base64,test",
            Ordem = 0,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Imagens.AddAsync(imagem);
        await context.SaveChangesAsync();

        var service = new ImageService(context, _loggerMock.Object, _storageProviderMock.Object);

        // Act
        var result = await service.RemoverImagemAsync(imagem.Id);

        // Assert
        result.Success.Should().BeTrue();
        var imagemRemovida = await context.Imagens.FindAsync(imagem.Id);
        imagemRemovida.Should().BeNull();
    }

    [Fact]
    public async Task RemoverImagensEntidadeAsync_DeveRemoverTodasImagensDaEntidade()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var entidadeId = Guid.NewGuid();

        var imagem1 = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Banner",
            EntidadeId = entidadeId,
            Categoria = "principal",
            ImagemUrl = "data:image/png;base64,test1",
            Ordem = 0,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var imagem2 = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Banner",
            EntidadeId = entidadeId,
            Categoria = "galeria",
            ImagemUrl = "data:image/png;base64,test2",
            Ordem = 1,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Imagens.AddRangeAsync(imagem1, imagem2);
        await context.SaveChangesAsync();

        var service = new ImageService(context, _loggerMock.Object, _storageProviderMock.Object);

        // Act
        var result = await service.RemoverImagensEntidadeAsync("Banner", entidadeId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(2);
        
        var imagensRestantes = await service.ListarImagensAsync("Banner", entidadeId);
        imagensRestantes.Should().BeEmpty();
    }

    // Helper para criar bytes de PNG válido
    private static byte[] CreateValidPngBytes()
    {
        // PNG mínimo 1x1 pixel transparente
        return new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG signature
            0x00, 0x00, 0x00, 0x0D, // IHDR length
            0x49, 0x48, 0x44, 0x52, // IHDR
            0x00, 0x00, 0x00, 0x01, // width: 1
            0x00, 0x00, 0x00, 0x01, // height: 1
            0x08, 0x06, 0x00, 0x00, 0x00, // bit depth, color type, etc
            0x1F, 0x15, 0xC4, 0x89, // CRC
            0x00, 0x00, 0x00, 0x0A, // IDAT length
            0x49, 0x44, 0x41, 0x54, // IDAT
            0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00, 0x05, 0x00, 0x01,
            0x0D, 0x0A, 0x2D, 0xB4, // CRC
            0x00, 0x00, 0x00, 0x00, // IEND length
            0x49, 0x45, 0x4E, 0x44, // IEND
            0xAE, 0x42, 0x60, 0x82  // CRC
        };
    }
}
