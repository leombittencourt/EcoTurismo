using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Uploads.Atrativos;

public class ReordenarImagensValidator : Validator<ReordenarImagensRequest>
{
    public ReordenarImagensValidator()
    {
        RuleFor(x => x.AtrativoId)
            .NotEmpty()
            .WithMessage("O ID do atrativo é obrigatório.");

        RuleFor(x => x.Imagens)
            .NotEmpty()
            .WithMessage("É necessário enviar pelo menos uma imagem para reordenar.");

        RuleForEach(x => x.Imagens)
            .Must(img => !string.IsNullOrWhiteSpace(img.Id))
            .WithMessage("Cada imagem deve ter um ID válido.");

        RuleForEach(x => x.Imagens)
            .Must(img => img.Ordem > 0)
            .WithMessage("A ordem deve ser um número positivo.");
    }
}
