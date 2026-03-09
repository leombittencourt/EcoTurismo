using EcoTurismo.Api.Authorization;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Enums;
using EcoTurismo.Infra.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class DeleteQuiosqueEndpoint : EndpointWithoutRequest
{
    private readonly IQuiosqueService _service;
    private readonly EcoTurismoDbContext _db;

    public DeleteQuiosqueEndpoint(IQuiosqueService service, EcoTurismoDbContext db)
    {
        _service = service;
        _db = db;
    }

    public override void Configure()
    {
        Delete("/api/quiosques/{Id}");
        Policies(RolePolicies.AdminOrBalnearioPolicy); // Apenas Admin ou Balneário pode deletar quiosques
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var possuiReservaAtivaOuFutura = await _db.Reservas.AnyAsync(r =>
            r.QuiosqueId == id &&
            (r.Status == ReservaStatus.Confirmada ||
             r.Status == ReservaStatus.EmAndamento ||
             r.Status == ReservaStatus.Validada) &&
            (r.DataFim ?? r.Data) >= hoje,
            ct);

        if (possuiReservaAtivaOuFutura)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            await HttpContext.Response.WriteAsJsonAsync(new
            {
                success = false,
                errorMessage = "Quiosque possui reservas ativas/futuras vinculadas. Inative o quiosque em vez de excluir."
            }, ct);
            return;
        }

        var ok = await _service.ExcluirAsync(id);

        if (!ok)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
