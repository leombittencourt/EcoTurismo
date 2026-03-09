using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Quiosques;

public class AcaoAdministrativaQuiosqueValidator : Validator<AcaoAdministrativaQuiosqueRequest>
{
    private static readonly string[] AcoesPermitidas = ["inativar", "editar", "desvincular_reservas", "excluir"];

    public AcaoAdministrativaQuiosqueValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id e obrigatorio");

        RuleFor(x => x.Acao)
            .NotEmpty()
            .WithMessage("Acao e obrigatoria")
            .Must(a => AcoesPermitidas.Contains(a.Trim().ToLowerInvariant()))
            .WithMessage("Acao invalida. Use: inativar, editar, desvincular_reservas, excluir");

        RuleFor(x => x.Motivo)
            .NotEmpty()
            .WithMessage("Motivo e obrigatorio")
            .MaximumLength(1000)
            .WithMessage("Motivo deve ter no maximo 1000 caracteres");
    }
}
