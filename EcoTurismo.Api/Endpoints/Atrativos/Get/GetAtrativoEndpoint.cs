using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class GetAtrativoEndpoint : EndpointWithoutRequest<AtrativoDto>
{
    private readonly EcoTurismoDbContext _db;

    public GetAtrativoEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/atrativos/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");

        var dataHoje = DateOnly.FromDateTime(DateTime.Today);

        var atrativo = await _db.Atrativos
            .Where(a => a.Id == id)
            .FirstOrDefaultAsync(ct);

        if (atrativo is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        // Buscar todas as imagens do atrativo
        var imagens = await _db.Imagens
            .Where(i => i.EntidadeTipo == "Atrativo" && i.EntidadeId == id)
            .OrderBy(i => i.Ordem)
            .Select(i => new ImagemAtrativoDto(
                i.Id.ToString(),
                i.ImagemUrl,
                i.Ordem,
                i.Categoria == "principal",
                null // Descrição pode ser extraída de MetadadosJson se necessário
            ))
            .ToListAsync(ct);

        // Calcular ocupação atual
        var ocupacaoAtual = await _db.Reservas
            .Where(r => r.AtrativoId == id && 
                       r.Data == dataHoje && 
                       (r.Status == ReservaStatus.Confirmada || 
                        r.Status == ReservaStatus.EmAndamento || 
                        r.Status == ReservaStatus.Validada))
            .SumAsync(r => r.QuantidadePessoas, ct);

        var imagemPrincipal = imagens.FirstOrDefault(i => i.Principal)?.Url 
                           ?? imagens.FirstOrDefault()?.Url;

        var dto = new AtrativoDto(
            atrativo.Id,
            atrativo.Nome,
            atrativo.Tipo,
            atrativo.MunicipioId,
            atrativo.CapacidadeMaxima,
            ocupacaoAtual,
            atrativo.Status,
            atrativo.Descricao,
            atrativo.Endereco,
            atrativo.Latitude,
            atrativo.Longitude,
            atrativo.MapUrl,
            imagemPrincipal,
            imagens
        );

        await Send.OkAsync(dto, ct);
    }
}
