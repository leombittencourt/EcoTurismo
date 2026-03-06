using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class ListAtrativosEndpoint : Endpoint<ListAtrativosRequest, PagedResponse<AtrativoDto>>
{
    private readonly EcoTurismoDbContext _db;

    public ListAtrativosEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/atrativos-municipio/{municipioId}");
        AllowAnonymous();
        Description(d => d
            .WithTags("Atrativos")
            .WithSummary("Lista atrativos com paginação")
            .WithDescription("Retorna lista paginada de atrativos. Suporta filtros por município, status, tipo e busca textual.")
            .Produces<PagedResponse<AtrativoDto>>(200));
    }

    public override async Task HandleAsync(ListAtrativosRequest req, CancellationToken ct)
    {
        var query = _db.Atrativos.AsQueryable();

        // Filtro por município (route param ou query param)
        var municipioIdRoute = Route<Guid?>("municipioId");
        var municipioId = municipioIdRoute ?? req.MunicipioId;

        if (municipioId.HasValue && municipioId.Value != Guid.Empty)
            query = query.Where(a => a.MunicipioId == municipioId.Value);

        // Filtro por status
        if (!string.IsNullOrWhiteSpace(req.Status))
            query = query.Where(a => a.Status == req.Status);

        // Filtro por tipo
        if (!string.IsNullOrWhiteSpace(req.Tipo))
        {
            if (Enum.TryParse<TipoAtrativo>(req.Tipo, true, out var tipo))
                query = query.Where(a => a.Tipo == tipo);
        }

        // Busca textual (nome ou descrição)
        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            var searchLower = req.Search.ToLower();
            query = query.Where(a => 
                a.Nome.ToLower().Contains(searchLower) || 
                (a.Descricao != null && a.Descricao.ToLower().Contains(searchLower)));
        }

        // Contar total antes da paginação
        var totalItems = await query.CountAsync(ct);

        var dataHoje = DateOnly.FromDateTime(DateTime.Today);

        // Aplicar ordenação estável e paginação
        var items = await query
            .OrderBy(a => a.Nome)
            .ThenBy(a => a.Id) // Ordenação secundária para garantir estabilidade
            .Skip(req.Skip)
            .Take(req.PageSize)
            .Select(a => new AtrativoDto(
                a.Id,
                a.Nome,
                a.Tipo,
                a.MunicipioId,
                a.CapacidadeMaxima,
                // Calcular ocupação atual baseada nas reservas ativas de hoje
                a.Reservas
                    .Where(r => r.Data == dataHoje && 
                               (r.Status == ReservaStatus.Confirmada || 
                                r.Status == ReservaStatus.EmAndamento || 
                                r.Status == ReservaStatus.Validada))
                    .Sum(r => r.QuantidadePessoas),
                a.Status,
                a.Descricao,
                a.Endereco,
                a.Latitude,
                a.Longitude,
                a.MapUrl,
                _db.Imagens
                    .Where(i => i.EntidadeTipo == "Atrativo" && i.EntidadeId == a.Id && i.Categoria == "principal")
                    .Select(i => i.ImagemUrl)
                    .FirstOrDefault(),
                null // Imagens não são retornadas na listagem por performance
            ))
            .ToListAsync(ct);

        var response = new PagedResponse<AtrativoDto>(items, req.Page, req.PageSize, totalItems);

        await Send.OkAsync(response, ct);
    }
}
