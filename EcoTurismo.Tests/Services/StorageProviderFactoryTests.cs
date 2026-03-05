using EcoTurismo.Application.Services;
using EcoTurismo.Application.Services.Storage;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace EcoTurismo.Tests.Services;

public class StorageProviderFactoryTests
{
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;

    public StorageProviderFactoryTests()
    {
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerFactoryMock
            .Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
    }

    [Fact]
    public void Create_DeveCriarBase64ProviderQuandoNaoConfigurado()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock
            .Setup(x => x["Storage:Provider"])
            .Returns((string?)null);

        var factory = new StorageProviderFactory(
            configurationMock.Object,
            _loggerFactoryMock.Object
        );

        // Act
        var provider = factory.Create();

        // Assert
        provider.Should().BeOfType<Base64StorageProvider>();
        provider.ProviderName.Should().Be("base64");
    }

    [Fact]
    public void Create_DeveCriarBase64ProviderQuandoConfiguradoExplicitamente()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock
            .Setup(x => x["Storage:Provider"])
            .Returns("base64");

        var factory = new StorageProviderFactory(
            configurationMock.Object,
            _loggerFactoryMock.Object
        );

        // Act
        var provider = factory.Create();

        // Assert
        provider.Should().BeOfType<Base64StorageProvider>();
    }

    [Fact]
    public void Create_DeveCriarOCIProviderQuandoConfigurado()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock
            .Setup(x => x["Storage:Provider"])
            .Returns("oci");

        var factory = new StorageProviderFactory(
            configurationMock.Object,
            _loggerFactoryMock.Object
        );

        // Act
        var provider = factory.Create();

        // Assert
        provider.Should().BeOfType<OCIStorageProvider>();
        provider.ProviderName.Should().Be("oci");
    }

    [Fact]
    public void Create_DeveLancarExcecaoParaProviderInvalido()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock
            .Setup(x => x["Storage:Provider"])
            .Returns("provider-invalido");

        var factory = new StorageProviderFactory(
            configurationMock.Object,
            _loggerFactoryMock.Object
        );

        // Act
        var act = () => factory.Create();

        // Assert
        act.Should().Throw<NotSupportedException>()
            .WithMessage("*provider-invalido*");
    }

    [Theory]
    [InlineData("base64")]
    [InlineData("BASE64")]
    [InlineData("Base64")]
    public void Create_DeveSerCaseInsensitive(string providerName)
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock
            .Setup(x => x["Storage:Provider"])
            .Returns(providerName);

        var factory = new StorageProviderFactory(
            configurationMock.Object,
            _loggerFactoryMock.Object
        );

        // Act
        var provider = factory.Create();

        // Assert
        provider.Should().BeOfType<Base64StorageProvider>();
    }
}
