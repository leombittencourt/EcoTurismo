using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class CreateQuiosqueRequest
{
    public Guid? AtrativoId { get; set; }
    public int Numero { get; set; }
    public bool TemChurrasqueira { get; set; }
    public string Status { get; set; } = "disponivel";
    public int PosicaoX { get; set; }
    public int PosicaoY { get; set; }
}

public class CreateQuiosqueValidator : Validator<CreateQuiosqueRequest>
{
    public CreateQuiosqueValidator()
    {
        RuleFor(x => x.Numero)
            .GreaterThan(0).WithMessage("Número deve ser maior que zero");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório")
            .MaximumLength(15);
    }
}

public class CreateQuiosqueEndpoint : Endpoint<CreateQuiosqueRequest, QuiosqueDto>
{
    private readonly IQuiosqueService _service;

    public CreateQuiosqueEndpoint(IQuiosqueService service) => _service = service;

    public override void Configure()
    {
        Post("/api/quiosques");
        Roles("admin", "prefeitura", "balneario");
    }

    public override async Task HandleAsync(CreateQuiosqueRequest req, CancellationToken ct)
    {
        var dto = new QuiosqueCreateRequest
        {
            AtrativoId = req.AtrativoId,
            Numero = req.Numero,
            TemChurrasqueira = req.TemChurrasqueira,
            Status = req.Status,
            PosicaoX = req.PosicaoX,
            PosicaoY = req.PosicaoY,
        };

        var result = await _service.CriarAsync(dto);
        await Send.CreatedAtAsync<GetQuiosqueEndpoint>(new { id = result.Id }, result, cancellation: ct);
    }
}
