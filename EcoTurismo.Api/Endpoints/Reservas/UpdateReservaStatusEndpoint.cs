using EcoTurismo.Application.Interfaces;
using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class UpdateReservaStatusRequest
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpdateReservaStatusValidator : Validator<UpdateReservaStatusRequest>
{
    public UpdateReservaStatusValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório")
            .MaximumLength(15);
    }
}

public class UpdateReservaStatusEndpoint : Endpoint<UpdateReservaStatusRequest>
{
    private readonly IReservaService _service;

    public UpdateReservaStatusEndpoint(IReservaService service) => _service = service;

    public override void Configure()
    {
        Put("/api/reservas/{Id}/status");
        Roles("admin", "prefeitura", "balneario");
    }

    public override async Task HandleAsync(UpdateReservaStatusRequest req, CancellationToken ct)
    {
        var ok = await _service.AtualizarStatusAsync(req.Id, req.Status);

        if (!ok)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
