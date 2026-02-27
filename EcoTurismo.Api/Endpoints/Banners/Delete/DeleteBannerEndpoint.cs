using EcoTurismo.Api.Authorization;
using EcoTurismo.Infra.Data;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Banners;

public class DeleteBannerEndpoint : Endpoint<DeleteBannerRequest>
{
    private readonly EcoTurismoDbContext _db;

    public DeleteBannerEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Delete("/api/banners/{Id}");
        Policies(RolePolicies.AdminPolicy);
    }

    public override async Task HandleAsync(DeleteBannerRequest req, CancellationToken ct)
    {
        var b = await _db.Banners.FindAsync([req.Id], ct);

        if (b is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        _db.Banners.Remove(b);
        await _db.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}
