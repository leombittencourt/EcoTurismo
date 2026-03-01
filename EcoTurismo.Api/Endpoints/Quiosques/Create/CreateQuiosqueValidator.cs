using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class CreateQuiosqueValidator : Validator<CreateQuiosqueRequest>
{
    public CreateQuiosqueValidator()
    {
        RuleFor(x => x.Numero)
            .GreaterThan(0).WithMessage("Número deve ser maior que zero");

        RuleFor(x => x.Status)
            .GreaterThanOrEqualTo(0).WithMessage("Status é obrigatório");
    }
}
