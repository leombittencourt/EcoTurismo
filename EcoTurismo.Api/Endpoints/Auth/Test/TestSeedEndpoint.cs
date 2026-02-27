using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Auth;

public class TestSeedEndpoint : EndpointWithoutRequest
{
    private readonly EcoTurismoDbContext _db;

    public TestSeedEndpoint(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/api/auth/test-seed");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var usuarios = await _db.Usuarios
            .Include(u => u.Role)
            .Select(u => new
            {
                u.Id,
                u.Nome,
                u.Email,
                Role = u.Role.Name,
                u.Ativo,
                PasswordHashPreview = u.PasswordHash.Substring(0, Math.Min(20, u.PasswordHash.Length)) + "..."
            })
            .ToListAsync(ct);

        var response = new
        {
            total = usuarios.Count,
            usuarios = usuarios,
            mensagem = usuarios.Count == 0 
                ? "⚠️ Nenhum usuário encontrado. Execute o seed." 
                : "✅ Usuários encontrados!"
        };

        await Send.OkAsync(response, ct);
    }
}
