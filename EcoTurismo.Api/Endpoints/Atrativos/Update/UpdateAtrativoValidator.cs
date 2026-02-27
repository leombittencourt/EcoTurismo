using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Atrativos;

public class UpdateAtrativoValidator : Validator<UpdateAtrativoRequest>
{
    public UpdateAtrativoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Nome)
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres")
            .When(x => x.Nome is not null);

        RuleFor(x => x.Tipo)
            .MaximumLength(20).WithMessage("Tipo deve ter no máximo 20 caracteres")
            .When(x => x.Tipo is not null);

        RuleFor(x => x.Status)
            .MaximumLength(20).WithMessage("Status deve ter no máximo 20 caracteres")
            .When(x => x.Status is not null);
    }
}
