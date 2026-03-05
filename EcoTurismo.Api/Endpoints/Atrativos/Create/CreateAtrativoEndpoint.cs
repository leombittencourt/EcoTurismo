using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Atrativos.Create;

/// <summary>
/// Endpoint para criar um novo atrativo turístico
/// </summary>
public class CreateAtrativoEndpoint : Endpoint<AtrativoCreateRequest, AtrativoDto>
{
    private readonly EcoTurismoDbContext _db;

    public CreateAtrativoEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Post("/api/atrativos");
        Policies(RolePolicies.AdminOrPrefeituraPolicy);
        Summary(s =>
        {
            s.Summary = "Cria um novo atrativo";
            s.Description = "Cria um novo atrativo turístico no sistema. Tipos disponíveis: Balneario, Cachoeira, Trilha, Parque, FazendaEcoturismo";
            s.ExampleRequest = new AtrativoCreateRequest
            {
                MunicipioId = Guid.NewGuid(),
                Nome = "Balneário das Águas Claras",
                Tipo = TipoAtrativo.Balneario,
                Descricao = "Balneário com piscinas naturais e infraestrutura completa",
                Imagem = "data:image/png;base64,...",
                CapacidadeMaxima = 500
            };
        });
    }

    public override async Task HandleAsync(AtrativoCreateRequest req, CancellationToken ct)
    {
        // Verificar se o município existe
        var municipioExiste = await _db.Municipios
            .AnyAsync(m => m.Id == req.MunicipioId, ct);

        if (!municipioExiste)
        {
            await Send.NotFoundAsync(ct);
            ThrowError("Município não encontrado");
        }

        // Verificar se já existe um atrativo com o mesmo nome no município
        var atrativoExiste = await _db.Atrativos
            .AnyAsync(a => a.MunicipioId == req.MunicipioId && 
                          a.Nome.ToLower() == req.Nome.ToLower(), ct);

        if (atrativoExiste)
        {
            ThrowError("Já existe um atrativo com este nome neste município");
        }

        // Criar novo atrativo
        var atrativo = new Atrativo
        {
            Id = Guid.NewGuid(),
            MunicipioId = req.MunicipioId,
            Nome = req.Nome,
            Tipo = req.Tipo,
            Descricao = req.Descricao,
            Endereco = req.Endereco,
            Latitude = req.Latitude,
            Longitude = req.Longitude,
            MapUrl = req.MapUrl,
            CapacidadeMaxima = req.CapacidadeMaxima,
            OcupacaoAtual = 0,
            Status = "ativo",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _db.Atrativos.Add(atrativo);
        await _db.SaveChangesAsync(ct);

        // Retornar DTO com tipo como string
        var dto = new AtrativoDto(
            atrativo.Id,
            atrativo.Nome,
            atrativo.Tipo,
            atrativo.MunicipioId,
            atrativo.CapacidadeMaxima,
            atrativo.OcupacaoAtual,
            atrativo.Status,
            atrativo.Descricao,
            atrativo.Endereco,
            atrativo.Latitude,
            atrativo.Longitude,
            atrativo.MapUrl,
            null,
            null
        );

        await Send.CreatedAtAsync<GetAtrativoEndpoint>(
            routeValues: new { Id = atrativo.Id },
            responseBody: dto,
            cancellation: ct);
    }
}
