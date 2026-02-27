using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Reservas;

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
