using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class UpdateQuiosqueValidator : Validator<UpdateQuiosqueRequest>
{
    public UpdateQuiosqueValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Status)
              .GreaterThanOrEqualTo(0).WithMessage("Status deve ser maior ou igual a 0")
            .When(x => x.Status is not null);
    }
}
