using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Reservas;

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
