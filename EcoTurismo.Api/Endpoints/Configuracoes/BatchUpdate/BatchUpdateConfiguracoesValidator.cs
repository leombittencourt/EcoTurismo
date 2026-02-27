using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Configuracoes;

public class BatchUpdateConfiguracoesValidator : Validator<BatchUpdateConfiguracoesRequest>
{
    public BatchUpdateConfiguracoesValidator()
    {
        RuleFor(x => x.Configs)
            .NotEmpty().WithMessage("Configs não pode estar vazio");

        RuleForEach(x => x.Configs)
            .ChildRules(c =>
            {
                c.RuleFor(x => x.Chave)
                    .NotEmpty().WithMessage("Chave é obrigatória");
            });
    }
}
