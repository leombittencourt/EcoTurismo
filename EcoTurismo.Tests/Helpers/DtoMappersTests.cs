using EcoTurismo.Api.Helpers;
using EcoTurismo.Domain.Entities;
using FluentAssertions;

namespace EcoTurismo.Tests.Helpers;

public class DtoMappersTests
{
    [Fact]
    public void ToDto_Imagem_DeveConverterCorretamente()
    {
        // Arrange
        var imagem = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Banner",
            EntidadeId = Guid.NewGuid(),
            Categoria = "principal",
            ImagemUrl = "data:image/png;base64,test",
            ThumbnailUrl = "data:image/png;base64,thumb",
            StorageProvider = "base64",
            Ordem = 0,
            MetadadosJson = @"{""nomeArquivo"":""test.png"",""tamanhoBytes"":1024}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var dto = imagem.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(imagem.Id);
        dto.EntidadeTipo.Should().Be("Banner");
        dto.Categoria.Should().Be("principal");
        dto.ImagemUrl.Should().Be(imagem.ImagemUrl);
        dto.ThumbnailUrl.Should().Be(imagem.ThumbnailUrl);
        dto.StorageProvider.Should().Be("base64");
        dto.Metadados.Should().NotBeNull();
    }

    [Fact]
    public void ToDto_ImagemNull_DeveRetornarNull()
    {
        // Arrange
        Imagem? imagem = null;

        // Act
        var dto = imagem.ToDto();

        // Assert
        dto.Should().BeNull();
    }

    [Fact]
    public void ToDto_Banner_DeveConverterComImagemIncluida()
    {
        // Arrange
        var bannerId = Guid.NewGuid();
        var imagemId = Guid.NewGuid();

        var imagem = new Imagem
        {
            Id = imagemId,
            EntidadeTipo = "Banner",
            EntidadeId = bannerId,
            Categoria = "principal",
            ImagemUrl = "data:image/png;base64,test",
            Ordem = 0,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var banner = new Banner
        {
            Id = bannerId,
            MunicipioId = Guid.NewGuid(),
            ImagemId = imagemId,
            Titulo = "Banner Teste",
            Subtitulo = "Subtítulo",
            Link = "https://example.com",
            Ordem = 1,
            Ativo = true,
            Imagem = imagem,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var dto = banner.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(bannerId);
        dto.Titulo.Should().Be("Banner Teste");
        dto.Imagem.Should().NotBeNull();
        dto.Imagem!.Id.Should().Be(imagemId);
        dto.Imagem.ImagemUrl.Should().Be("data:image/png;base64,test");
    }

    [Fact]
    public void ToDto_Banner_DeveFuncionarComImagemNull()
    {
        // Arrange
        var banner = new Banner
        {
            Id = Guid.NewGuid(),
            MunicipioId = Guid.NewGuid(),
            ImagemId = null,
            Titulo = "Banner Sem Imagem",
            Ordem = 1,
            Ativo = true,
            Imagem = null,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var dto = banner.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Titulo.Should().Be("Banner Sem Imagem");
        dto.Imagem.Should().BeNull();
    }

    [Fact]
    public void ToDto_Municipio_DeveConverterComTodosLogos()
    {
        // Arrange
        var municipioId = Guid.NewGuid();

        var logo = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Municipio",
            EntidadeId = municipioId,
            Categoria = "logo_geral",
            ImagemUrl = "data:image/png;base64,logo",
            Ordem = 0,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var logoLogin = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Municipio",
            EntidadeId = municipioId,
            Categoria = "logo_login",
            ImagemUrl = "data:image/png;base64,login",
            Ordem = 0,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var logoPublico = new Imagem
        {
            Id = Guid.NewGuid(),
            EntidadeTipo = "Municipio",
            EntidadeId = municipioId,
            Categoria = "logo_publico",
            ImagemUrl = "data:image/png;base64,publico",
            Ordem = 0,
            MetadadosJson = "{}",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var municipio = new Municipio
        {
            Id = municipioId,
            Nome = "Município Teste",
            Uf = "SC",
            LogoId = logo.Id,
            LogoTelaLoginId = logoLogin.Id,
            LogoAreaPublicaId = logoPublico.Id,
            Logo = logo,
            LogoTelaLogin = logoLogin,
            LogoAreaPublica = logoPublico,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var dto = municipio.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Nome.Should().Be("Município Teste");
        dto.Uf.Should().Be("SC");
        dto.Logo.Should().NotBeNull();
        dto.LogoTelaLogin.Should().NotBeNull();
        dto.LogoAreaPublica.Should().NotBeNull();
        dto.Logo!.Categoria.Should().Be("logo_geral");
        dto.LogoTelaLogin!.Categoria.Should().Be("logo_login");
        dto.LogoAreaPublica!.Categoria.Should().Be("logo_publico");
    }

    [Fact]
    public void ToDto_Municipio_DeveFuncionarSemLogos()
    {
        // Arrange
        var municipio = new Municipio
        {
            Id = Guid.NewGuid(),
            Nome = "Município Sem Logos",
            Uf = "PR",
            LogoId = null,
            LogoTelaLoginId = null,
            LogoAreaPublicaId = null,
            Logo = null,
            LogoTelaLogin = null,
            LogoAreaPublica = null,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var dto = municipio.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Nome.Should().Be("Município Sem Logos");
        dto.Logo.Should().BeNull();
        dto.LogoTelaLogin.Should().BeNull();
        dto.LogoAreaPublica.Should().BeNull();
    }
}
