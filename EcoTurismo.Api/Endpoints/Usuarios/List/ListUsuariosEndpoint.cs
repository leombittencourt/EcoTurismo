using EcoTurismo.Api.Authorization;
using EcoTurismo.Api.Endpoints.Usuarios.List;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Usuarios;

public class ListUsuariosEndpoint : Endpoint<ListUsuariosRequest, PagedResponse<UsuarioListItem>>
{
    private readonly EcoTurismoDbContext _db;

    public ListUsuariosEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/usuarios");
        Policies(RolePolicies.AdminPolicy);
        Description(d => d
            .WithTags("Usuários")
            .WithSummary("Lista usuários com paginação")
            .WithDescription("Retorna lista paginada de usuários. Suporta filtros por município, role, status ativo e busca textual.")
            .Produces<PagedResponse<UsuarioListItem>>(200)
            .Produces(401)
            .Produces(403));
    }

    public override async Task HandleAsync(ListUsuariosRequest req, CancellationToken ct)
    {
        var query = _db.Usuarios.AsQueryable();

        // Filtro por município
        if (req.MunicipioId.HasValue)
            query = query.Where(u => u.MunicipioId == req.MunicipioId.Value);

        // Filtro por role
        if (req.RoleId.HasValue)
            query = query.Where(u => u.RoleId == req.RoleId.Value);

        // Filtro por status ativo
        if (req.Ativo.HasValue)
            query = query.Where(u => u.Ativo == req.Ativo.Value);

        // Busca textual (nome ou email)
        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            var searchLower = req.Search.ToLower();
            query = query.Where(u => 
                u.Nome.ToLower().Contains(searchLower) || 
                u.Email.ToLower().Contains(searchLower));
        }

        // Contar total antes da paginação
        var totalItems = await query.CountAsync(ct);

        // Aplicar ordenação estável e paginação
        var items = await query
            .Include(u => u.Role)
            .OrderBy(u => u.Nome)
            .ThenBy(u => u.Id) // Ordenação secundária para garantir estabilidade
            .Skip(req.Skip)
            .Take(req.PageSize)
            .Select(u => new UsuarioListItem(
                u.Id,
                u.Nome,
                u.Email,
                u.Role.Name,
                u.Ativo
            ))
            .ToListAsync(ct);

        var response = new PagedResponse<UsuarioListItem>(items, req.Page, req.PageSize, totalItems);

        await Send.OkAsync(response, ct);
    }
}
