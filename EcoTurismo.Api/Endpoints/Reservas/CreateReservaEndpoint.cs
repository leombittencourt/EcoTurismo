using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class CreateReservaRequest
{
    public Guid AtrativoId { get; set; }
    public Guid? QuiosqueId { get; set; }
    public string NomeVisitante { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string CidadeOrigem { get; set; } = string.Empty;
    public string UfOrigem { get; set; } = string.Empty;
    public string Tipo { get; set; } = "day_use";
    public DateOnly Data { get; set; }
    public DateOnly? DataFim { get; set; }
    public int QuantidadePessoas { get; set; } = 1;
}

public class CreateReservaValidator : Validator<CreateReservaRequest>
{
    public CreateReservaValidator()
    {
        RuleFor(x => x.AtrativoId)
            .NotEmpty().WithMessage("AtrativoId é obrigatório");

        RuleFor(x => x.NomeVisitante)
            .NotEmpty().WithMessage("Nome do visitante é obrigatório")
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(200);

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .MaximumLength(14);

        RuleFor(x => x.CidadeOrigem)
            .NotEmpty().WithMessage("Cidade de origem é obrigatória")
            .MaximumLength(100);

        RuleFor(x => x.UfOrigem)
            .NotEmpty().WithMessage("UF de origem é obrigatória")
            .MaximumLength(2);

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("Tipo é obrigatório")
            .MaximumLength(10);

        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("Data é obrigatória");

        RuleFor(x => x.QuantidadePessoas)
            .GreaterThan(0).WithMessage("Quantidade de pessoas deve ser maior que zero");
    }
}

public class CreateReservaEndpoint : Endpoint<CreateReservaRequest, ReservaDto>
{
    private readonly IReservaService _service;

    public CreateReservaEndpoint(IReservaService service) => _service = service;

    public override void Configure()
    {
        Post("/api/reservas");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateReservaRequest req, CancellationToken ct)
    {
        var dto = new ReservaCreateRequest
        {
            AtrativoId = req.AtrativoId,
            QuiosqueId = req.QuiosqueId,
            NomeVisitante = req.NomeVisitante,
            Email = req.Email,
            Cpf = req.Cpf,
            CidadeOrigem = req.CidadeOrigem,
            UfOrigem = req.UfOrigem,
            Tipo = req.Tipo,
            Data = req.Data,
            DataFim = req.DataFim,
            QuantidadePessoas = req.QuantidadePessoas,
        };

        var reserva = await _service.CriarAsync(dto);
        await Send.CreatedAtAsync<GetReservaEndpoint>(new { id = reserva.Id }, reserva, cancellation: ct);
    }
}
