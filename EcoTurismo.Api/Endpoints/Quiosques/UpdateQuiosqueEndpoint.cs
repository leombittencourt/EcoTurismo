using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class UpdateQuiosqueRequest
{
    public Guid Id { get; set; }
    public int? Numero { get; set; }
    public bool? TemChurrasqueira { get; set; }
    public string? Status { get; set; }
    public int? PosicaoX { get; set; }
    public int? PosicaoY { get; set; }
}

public class UpdateQuiosqueValidator : Validator<UpdateQuiosqueRequest>
{
    public UpdateQuiosqueValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Status)
            .MaximumLength(15).WithMessage("Status deve ter no máximo 15 caracteres")
            .When(x => x.Status is not null);
    }
}

public class UpdateQuiosqueEndpoint : Endpoint<UpdateQuiosqueRequest, QuiosqueDto>
{
    private readonly IQuiosqueService _service;

    public UpdateQuiosqueEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Put("/api/quiosques/{Id}");
        Roles("admin", "prefeitura", "balneario");
    }

    public override async Task HandleAsync(UpdateQuiosqueRequest req, CancellationToken ct)
    {
        var dto = new QuiosqueUpdateRequest
        {
            Numero = req.Numero,
            TemChurrasqueira = req.TemChurrasqueira,
            Status = req.Status,
            PosicaoX = req.PosicaoX,
            PosicaoY = req.PosicaoY,
        };

        var result = await _service.AtualizarAsync(req.Id, dto);

        if (result is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(result, ct);
    }
}
