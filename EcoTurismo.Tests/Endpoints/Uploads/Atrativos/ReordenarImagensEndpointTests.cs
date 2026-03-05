using EcoTurismo.Api.Endpoints.Uploads.Atrativos;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcoTurismo.Tests.Endpoints.Uploads.Atrativos;

public class ReordenarImagensEndpointTests : IDisposable
{
    private readonly EcoTurismoDbContext _db;
    private readonly ReordenarImagensEndpoint _endpoint;
    private readonly Guid _atrativoId;

    public ReordenarImagensEndpointTests()
    {
        var options = new DbContextOptionsBuilder<EcoTurismoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new EcoTurismoDbContext(options);
        _endpoint = new ReordenarImagensEndpoint(_db);

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
    public async Task Reordenar_ComOrdensValidas_DeveAtualizarOrdens()
    {
        // Arrange
        var imagens = new List<ImagemAtrativoDto>
        {
            new("id1", "data:image/jpeg;base64,abc", 1, true, "Foto 1"),
            new("id2", "data:image/jpeg;base64,def", 2, false, "Foto 2"),
            new("id3", "data:image/jpeg;base64,ghi", 3, false, "Foto 3")
        };

        var atrativo = await _db.Atrativos.FindAsync(_atrativoId);
        atrativo!.Imagens = JsonSerializer.Serialize(imagens);
        await _db.SaveChangesAsync();

        var request = new ReordenarImagensRequest
        {
            AtrativoId = _atrativoId,
            Imagens = new List<ImagemOrdemDto>
            {
                new("id3", 1), // id3 vai para primeira posição
                new("id1", 2), // id1 vai para segunda
                new("id2", 3)  // id2 vai para terceira
            }
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var atrativoAtualizado = await _db.Atrativos.FindAsync(_atrativoId);
        var imagensAtualizadas = JsonSerializer.Deserialize<List<ImagemAtrativoDto>>(atrativoAtualizado!.Imagens!);

        imagensAtualizadas!.First(i => i.Id == "id3").Ordem.Should().Be(1);
        imagensAtualizadas.First(i => i.Id == "id1").Ordem.Should().Be(2);
        imagensAtualizadas.First(i => i.Id == "id2").Ordem.Should().Be(3);
    }

    [Fact]
    public async Task Reordenar_ImagensDevemEstarOrdenadasApósReordenação()
    {
        // Arrange
        var imagens = new List<ImagemAtrativoDto>
        {
            new("id1", "data:image/jpeg;base64,abc", 1, true, "Foto 1"),
            new("id2", "data:image/jpeg;base64,def", 2, false, "Foto 2"),
            new("id3", "data:image/jpeg;base64,ghi", 3, false, "Foto 3"),
            new("id4", "data:image/jpeg;base64,jkl", 4, false, "Foto 4")
        };

        var atrativo = await _db.Atrativos.FindAsync(_atrativoId);
        atrativo!.Imagens = JsonSerializer.Serialize(imagens);
        await _db.SaveChangesAsync();

        var request = new ReordenarImagensRequest
        {
            AtrativoId = _atrativoId,
            Imagens = new List<ImagemOrdemDto>
            {
                new("id4", 1),
                new("id2", 2),
                new("id1", 3),
                new("id3", 4)
            }
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var atrativoAtualizado = await _db.Atrativos.FindAsync(_atrativoId);
        var imagensAtualizadas = JsonSerializer.Deserialize<List<ImagemAtrativoDto>>(atrativoAtualizado!.Imagens!);

        // Verificar se está ordenado
        var ordensSequenciais = imagensAtualizadas!.Select(i => i.Ordem).ToList();
        ordensSequenciais.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Reordenar_ReordenacaoParcial_DeveAtualizarApenasEspecificadas()
    {
        // Arrange
        var imagens = new List<ImagemAtrativoDto>
        {
            new("id1", "data:image/jpeg;base64,abc", 1, true, "Foto 1"),
            new("id2", "data:image/jpeg;base64,def", 2, false, "Foto 2"),
            new("id3", "data:image/jpeg;base64,ghi", 3, false, "Foto 3")
        };

        var atrativo = await _db.Atrativos.FindAsync(_atrativoId);
        atrativo!.Imagens = JsonSerializer.Serialize(imagens);
        await _db.SaveChangesAsync();

        var request = new ReordenarImagensRequest
        {
            AtrativoId = _atrativoId,
            Imagens = new List<ImagemOrdemDto>
            {
                new("id1", 10),
                new("id3", 5)
                // id2 não foi incluído, deve manter ordem 2
            }
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var atrativoAtualizado = await _db.Atrativos.FindAsync(_atrativoId);
        var imagensAtualizadas = JsonSerializer.Deserialize<List<ImagemAtrativoDto>>(atrativoAtualizado!.Imagens!);

        imagensAtualizadas!.First(i => i.Id == "id1").Ordem.Should().Be(10);
        imagensAtualizadas.First(i => i.Id == "id2").Ordem.Should().Be(2); // Não mudou
        imagensAtualizadas.First(i => i.Id == "id3").Ordem.Should().Be(5);
    }

    [Fact]
    public async Task Reordenar_ComIdInvalido_DeveLancarErro()
    {
        // Arrange
        var imagens = new List<ImagemAtrativoDto>
        {
            new("id1", "data:image/jpeg;base64,abc", 1, true, "Foto 1"),
            new("id2", "data:image/jpeg;base64,def", 2, false, "Foto 2")
        };

        var atrativo = await _db.Atrativos.FindAsync(_atrativoId);
        atrativo!.Imagens = JsonSerializer.Serialize(imagens);
        await _db.SaveChangesAsync();

        var request = new ReordenarImagensRequest
        {
            AtrativoId = _atrativoId,
            Imagens = new List<ImagemOrdemDto>
            {
                new("id1", 1),
                new("id-inexistente", 2)
            }
        };

        // Act & Assert
        var act = async () => await _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*inválidos*");
    }

    [Fact]
    public async Task Reordenar_AtrativoSemImagens_DeveLancarErro()
    {
        // Arrange
        var request = new ReordenarImagensRequest
        {
            AtrativoId = _atrativoId,
            Imagens = new List<ImagemOrdemDto>
            {
                new("id1", 1)
            }
        };

        // Act & Assert
        var act = async () => await _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*não possui imagens*");
    }

    [Fact]
    public async Task Reordenar_AtrativoInexistente_DeveLancarErro()
    {
        // Arrange
        var request = new ReordenarImagensRequest
        {
            AtrativoId = Guid.NewGuid(),
            Imagens = new List<ImagemOrdemDto>
            {
                new("id1", 1)
            }
        };

        // Act & Assert
        var act = async () => await _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task Reordenar_MantémImagemPrincipal_NãoDeveAlterarPrincipal()
    {
        // Arrange
        var imagens = new List<ImagemAtrativoDto>
        {
            new("id1", "data:image/jpeg;base64,abc", 1, true, "Foto 1"),
            new("id2", "data:image/jpeg;base64,def", 2, false, "Foto 2"),
            new("id3", "data:image/jpeg;base64,ghi", 3, false, "Foto 3")
        };

        var atrativo = await _db.Atrativos.FindAsync(_atrativoId);
        atrativo!.Imagens = JsonSerializer.Serialize(imagens);
        await _db.SaveChangesAsync();

        var request = new ReordenarImagensRequest
        {
            AtrativoId = _atrativoId,
            Imagens = new List<ImagemOrdemDto>
            {
                new("id3", 1),
                new("id2", 2),
                new("id1", 3)
            }
        };

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var atrativoAtualizado = await _db.Atrativos.FindAsync(_atrativoId);
        var imagensAtualizadas = JsonSerializer.Deserialize<List<ImagemAtrativoDto>>(atrativoAtualizado!.Imagens!);

        // id1 continua sendo principal, mesmo que tenha mudado de ordem
        imagensAtualizadas!.First(i => i.Id == "id1").Principal.Should().BeTrue();
    }

    public void Dispose()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }
}
