using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcoTurismo.Api.Endpoints.Profiles;

public class MeEndpoint : EndpointWithoutRequest<MeResponse>
{
    private readonly EcoTurismoDbContext _db;

    public MeEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Get("/api/profiles/me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var profile = await _db.Profiles
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.Id == Guid.Parse(userId), ct);

        if (profile is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new MeResponse
        {
            Id = profile.Id,
            Nome = profile.Nome,
            Email = profile.Email,
            Role = profile.Role.Name,
            MunicipioId = profile.MunicipioId,
            AtrativoId = profile.AtrativoId
        }, ct);
    }
}
