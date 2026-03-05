using EcoTurismo.Application.Services.Storage;
using FluentAssertions;

namespace EcoTurismo.Tests.Services.Storage;

public class Base64StorageProviderTests
{
    private readonly Base64StorageProvider _provider;

    public Base64StorageProviderTests()
    {
        _provider = new Base64StorageProvider();
    }

    [Fact]
    public void ProviderName_DeveRetornarBase64()
    {
        // Act
        var providerName = _provider.ProviderName;

        // Assert
        providerName.Should().Be("base64");
    }

    [Fact]
    public async Task SaveImageAsync_DeveRetornarDataUri()
    {
        // Arrange
        var imageBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG header
        var fileName = "test.png";
        var contentType = "image/png";

        // Act
        var result = await _provider.SaveImageAsync(imageBytes, fileName, contentType);

        // Assert
        result.Should().StartWith("data:image/png;base64,");
        result.Should().Contain(Convert.ToBase64String(imageBytes));
    }

    [Fact]
    public async Task GetImageBytesAsync_DeveRetornarBytesDeDataUri()
    {
        // Arrange
        var originalBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        var base64String = Convert.ToBase64String(originalBytes);
        var dataUri = $"data:image/png;base64,{base64String}";

        // Act
        var result = await _provider.GetImageBytesAsync(dataUri);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(originalBytes);
    }

    [Fact]
    public async Task GetImageBytesAsync_DeveRetornarNullQuandoNaoEhDataUri()
    {
        // Arrange
        var invalidUri = "https://example.com/image.png";

        // Act
        var result = await _provider.GetImageBytesAsync(invalidUri);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteImageAsync_DeveSempreRetornarTrue()
    {
        // Arrange
        var imageUrl = "data:image/png;base64,iVBORw0KGgo=";

        // Act
        var result = await _provider.DeleteImageAsync(imageUrl);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_DeveRetornarTrueParaDataUri()
    {
        // Arrange
        var dataUri = "data:image/png;base64,iVBORw0KGgo=";

        // Act
        var result = await _provider.ExistsAsync(dataUri);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_DeveRetornarFalseParaUrlNormal()
    {
        // Arrange
        var normalUrl = "https://example.com/image.png";

        // Act
        var result = await _provider.ExistsAsync(normalUrl);

        // Assert
        result.Should().BeFalse();
    }
}
