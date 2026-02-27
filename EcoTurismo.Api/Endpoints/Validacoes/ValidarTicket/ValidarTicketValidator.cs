using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Validacoes;

public class ValidarTicketValidator : Validator<ValidarTicketRequest>
{
    public ValidarTicketValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token é obrigatório")
            .MaximumLength(50);
    }
}
