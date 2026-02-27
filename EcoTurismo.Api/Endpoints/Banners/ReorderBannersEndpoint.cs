using EcoTurismo.Infra.Data;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Banners;

public class ReorderBannersRequest
{
    public List<ReorderItem> Itens { get; set; } = [];
}

public class ReorderItem
{
    public Guid Id { get; set; }
    public int Ordem { get; set; }
}

public class ReorderBannersEndpoint : Endpoint<ReorderBannersRequest>
{
    private readonly EcoTurismoDbContext _db;

    public ReorderBannersEndpoint(EcoTurismoDbContext db) => _db = db;

    public override void Configure()
    {
        Put("/api/banners/reorder");
        Roles("admin", "prefeitura");
    }

    public override async Task HandleAsync(ReorderBannersRequest req, CancellationToken ct)
    {
        foreach (var item in req.Itens)
        {
            var b = await _db.Banners.FindAsync([item.Id], ct);
            if (b is not null)
                b.Ordem = item.Ordem;
        }

        await _db.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}
