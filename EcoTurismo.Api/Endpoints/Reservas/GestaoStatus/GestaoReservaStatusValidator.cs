using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Reservas;

public class GestaoReservaStatusValidator : Validator<GestaoReservaStatusRequest>
{
    private static readonly string[] StatusPermitidos =
    [
        "confirmada",
        "em_andamento",
        "concluida",
        "cancelada",
        "validada",
        "nao_compareceu"
    ];

    public GestaoReservaStatusValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id e obrigatorio");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status e obrigatorio")
            .Must(s => StatusPermitidos.Contains(s.Trim().ToLowerInvariant()))
            .WithMessage("Status invalido para gestao da reserva");

        RuleFor(x => x.Motivo)
            .NotEmpty()
            .WithMessage("Motivo e obrigatorio")
            .MaximumLength(1000)
            .WithMessage("Motivo deve ter no maximo 1000 caracteres");
    }
}
