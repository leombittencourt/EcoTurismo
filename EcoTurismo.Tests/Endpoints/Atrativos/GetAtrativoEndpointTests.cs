using EcoTurismo.Api.Endpoints.Atrativos;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Tests.Helpers;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Tests.Endpoints.Atrativos;

public class GetAtrativoEndpointTests
{
    [Fact]
    public async Task HandleAsync_DeveRetornarAtrativoQuandoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        
        var municipio = new Municipio
        {
            Id = Guid.NewGuid(),
            Nome = "Município Teste",
            Uf = "SC",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var atrativo = new Atrativo
        {
            Id = Guid.NewGuid(),
            Nome = "Balneário Teste",
            Tipo = "balneario",
            MunicipioId = municipio.Id,
            CapacidadeMaxima = 100,
            OcupacaoAtual = 0,
            Status = "ativo",
            Descricao = "Descrição do balneário",
            Imagem = null,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Municipios.AddAsync(municipio);
        await context.Atrativos.AddAsync(atrativo);
        await context.SaveChangesAsync();

        var endpoint = Factory.Create<GetAtrativoEndpoint>(context);
        endpoint.HttpContext.Request.RouteValues["Id"] = atrativo.Id.ToString();

        // Act
        await endpoint.HandleAsync(default);
        var response = endpoint.Response;

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(atrativo.Id);
        response.Nome.Should().Be("Balneário Teste");
        response.CapacidadeMaxima.Should().Be(100);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarNotFoundQuandoNaoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var endpoint = Factory.Create<GetAtrativoEndpoint>(context);
        endpoint.HttpContext.Request.RouteValues["Id"] = Guid.NewGuid().ToString();

        // Act
        await endpoint.HandleAsync(default);

        // Assert
        endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task HandleAsync_DeveCalcularOcupacaoCorretamente()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        
        var municipio = new Municipio
        {
            Id = Guid.NewGuid(),
            Nome = "Município Teste",
            Uf = "SC",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var atrativo = new Atrativo
        {
            Id = Guid.NewGuid(),
            Nome = "Balneário Teste",
            Tipo = "balneario",
            MunicipioId = municipio.Id,
            CapacidadeMaxima = 100,
            OcupacaoAtual = 0,
            Status = "ativo",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var hoje = DateOnly.FromDateTime(DateTime.Today);

        // Reservas ativas
        var reserva1 = new Reserva
        {
            Id = Guid.NewGuid(),
            AtrativoId = atrativo.Id,
            NomeVisitante = "João",
            Email = "joao@test.com",
            Cpf = "12345678900",
            CidadeOrigem = "Cidade",
            UfOrigem = "SC",
            Data = hoje,
            QuantidadePessoas = 5,
            Status = ReservaStatus.Confirmada,
            Token = "TOKEN1",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var reserva2 = new Reserva
        {
            Id = Guid.NewGuid(),
            AtrativoId = atrativo.Id,
            NomeVisitante = "Maria",
            Email = "maria@test.com",
            Cpf = "98765432100",
            CidadeOrigem = "Cidade",
            UfOrigem = "SC",
            Data = hoje,
            QuantidadePessoas = 3,
            Status = ReservaStatus.EmAndamento,
            Token = "TOKEN2",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Reserva cancelada (não deve contar)
        var reserva3 = new Reserva
        {
            Id = Guid.NewGuid(),
            AtrativoId = atrativo.Id,
            NomeVisitante = "Pedro",
            Email = "pedro@test.com",
            Cpf = "11111111111",
            CidadeOrigem = "Cidade",
            UfOrigem = "SC",
            Data = hoje,
            QuantidadePessoas = 10,
            Status = ReservaStatus.Cancelada,
            Token = "TOKEN3",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await context.Municipios.AddAsync(municipio);
        await context.Atrativos.AddAsync(atrativo);
        await context.Reservas.AddRangeAsync(reserva1, reserva2, reserva3);
        await context.SaveChangesAsync();

        var endpoint = Factory.Create<GetAtrativoEndpoint>(context);
        endpoint.HttpContext.Request.RouteValues["Id"] = atrativo.Id.ToString();

        // Act
        await endpoint.HandleAsync(default);
        var response = endpoint.Response;

        // Assert
        response.Should().NotBeNull();
        response.OcupacaoAtual.Should().Be(8); // 5 + 3, cancelada não conta
    }
}
