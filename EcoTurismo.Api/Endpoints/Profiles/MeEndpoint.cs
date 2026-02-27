using EcoTurismo.Infra.Data;
using FastEndpoints;
using System.Security.Claims;

namespace EcoTurismo.Api.Endpoints.Profiles;

public class MeResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid? MunicipioId { get; set; }
    public Guid? AtrativoId { get; set; }
}

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

        var profile = await _db.Profiles.FindAsync([Guid.Parse(userId)], ct);

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
            Role = profile.Role,
            MunicipioId = profile.MunicipioId,
            AtrativoId = profile.AtrativoId
        }, ct);
    }
}
