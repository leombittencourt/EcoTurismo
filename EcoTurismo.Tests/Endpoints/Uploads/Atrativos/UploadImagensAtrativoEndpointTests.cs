using EcoTurismo.Api.Endpoints.Uploads.Atrativos;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace EcoTurismo.Tests.Endpoints.Uploads.Atrativos;

public class UploadImagensAtrativoEndpointTests : IDisposable
{
    private readonly EcoTurismoDbContext _db;
    private readonly UploadImagensAtrativoEndpoint _endpoint;
    private readonly Guid _atrativoId;

    public UploadImagensAtrativoEndpointTests()
    {
        var options = new DbContextOptionsBuilder<EcoTurismoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new EcoTurismoDbContext(options);
        _endpoint = new UploadImagensAtrativoEndpoint(_db);

        // Seed data
        _atrativoId = Guid.NewGuid();
        var municipio = new Municipio
        {
            Id = Guid.NewGuid(),
            Nome = "São Paulo",
            Uf = "SP"
        };

        var atrativo = new Atrativo
        {
            Id = _atrativoId,
            MunicipioId = municipio.Id,
            Nome = "Cachoeira do Sol",
            Tipo = "cachoeira",
            CapacidadeMaxima = 100,
            Status = "ativo"
        };

        _db.Municipios.Add(municipio);
        _db.Atrativos.Add(atrativo);
        _db.SaveChanges();
    }

    [Fact]
    public async Task Upload_ComUmaImagem_DeveAdicionarComSucesso()
    {
        // Arrange
        var file = CreateFakeImageFile("test.jpg", 1024);
        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = _atrativoId,
            Imagens = new[] { file },
            Descricoes = new[] { "Foto principal" }
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var atrativo = await _db.Atrativos.FindAsync(_atrativoId);
        atrativo!.Imagens.Should().NotBeNullOrEmpty();

        var imagens = JsonSerializer.Deserialize<List<ImagemAtrativoDto>>(atrativo.Imagens!);
        imagens.Should().HaveCount(1);
        imagens![0].Descricao.Should().Be("Foto principal");
        imagens[0].Principal.Should().BeTrue(); // Primeira imagem é principal
        imagens[0].Ordem.Should().Be(1);
        imagens[0].Url.Should().StartWith("data:image/jpeg;base64,");
    }

    [Fact]
    public async Task Upload_ComMultiplasImagens_DeveAdicionarTodas()
    {
        // Arrange
        var files = new[]
        {
            CreateFakeImageFile("foto1.jpg", 1024),
            CreateFakeImageFile("foto2.png", 2048),
            CreateFakeImageFile("foto3.jpg", 1500)
        };

        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = _atrativoId,
            Imagens = files,
            Descricoes = new[] { "Foto 1", "Foto 2", "Foto 3" }
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var atrativo = await _db.Atrativos.FindAsync(_atrativoId);
        var imagens = JsonSerializer.Deserialize<List<ImagemAtrativoDto>>(atrativo!.Imagens!);

        imagens.Should().HaveCount(3);
        imagens![0].Ordem.Should().Be(1);
        imagens[1].Ordem.Should().Be(2);
        imagens[2].Ordem.Should().Be(3);
    }

    [Fact]
    public async Task Upload_ComOrdensCustomizadas_DeveRespeitarOrdens()
    {
        // Arrange
        var files = new[]
        {
            CreateFakeImageFile("foto1.jpg", 1024),
            CreateFakeImageFile("foto2.jpg", 1024)
        };

        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = _atrativoId,
            Imagens = files,
            Ordens = new[] { 5, 3 }
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var atrativo = await _db.Atrativos.FindAsync(_atrativoId);
        var imagens = JsonSerializer.Deserialize<List<ImagemAtrativoDto>>(atrativo!.Imagens!);

        imagens![0].Ordem.Should().Be(5);
        imagens[1].Ordem.Should().Be(3);
    }

    [Fact]
    public async Task Upload_AdicionandoImagensExistentes_DeveManterAntigas()
    {
        // Arrange
        // Adicionar imagens existentes
        var imagensExistentes = new List<ImagemAtrativoDto>
        {
            new("id1", "data:image/jpeg;base64,abc", 1, true, "Antiga 1"),
            new("id2", "data:image/jpeg;base64,def", 2, false, "Antiga 2")
        };

        var atrativo = await _db.Atrativos.FindAsync(_atrativoId);
        atrativo!.Imagens = JsonSerializer.Serialize(imagensExistentes);
        await _db.SaveChangesAsync();

        var file = CreateFakeImageFile("nova.jpg", 1024);
        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = _atrativoId,
            Imagens = new[] { file }
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var atrativoAtualizado = await _db.Atrativos.FindAsync(_atrativoId);
        var imagens = JsonSerializer.Deserialize<List<ImagemAtrativoDto>>(atrativoAtualizado!.Imagens!);

        imagens.Should().HaveCount(3);
        imagens![0].Descricao.Should().Be("Antiga 1");
        imagens[1].Descricao.Should().Be("Antiga 2");
    }

    [Fact]
    public async Task Upload_ComLimiteExcedido_DeveLancarErro()
    {
        // Arrange
        // Adicionar 18 imagens existentes
        var imagensExistentes = Enumerable.Range(1, 18)
            .Select(i => new ImagemAtrativoDto($"id{i}", "data:image/jpeg;base64,abc", i, i == 1, $"Imagem {i}"))
            .ToList();

        var atrativo = await _db.Atrativos.FindAsync(_atrativoId);
        atrativo!.Imagens = JsonSerializer.Serialize(imagensExistentes);
        await _db.SaveChangesAsync();

        var files = new[]
        {
            CreateFakeImageFile("foto1.jpg", 1024),
            CreateFakeImageFile("foto2.jpg", 1024),
            CreateFakeImageFile("foto3.jpg", 1024)
        };

        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = _atrativoId,
            Imagens = files
        };

        // Act & Assert
        var act = async () => await _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*20 imagens*");
    }

    [Fact]
    public async Task Upload_AtrativoInexistente_DeveLancarErro()
    {
        // Arrange
        var file = CreateFakeImageFile("test.jpg", 1024);
        var request = new UploadImagensAtrativoRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new[] { file }
        };

        // Act & Assert
        var act = async () => await _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*não encontrado*");
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

    public void Dispose()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }
}
