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
            .MaximumLength(15).WithMessage("Status deve ter no máximo 15 caracteres")
            .When(x => x.Status is not null);
    }
}
