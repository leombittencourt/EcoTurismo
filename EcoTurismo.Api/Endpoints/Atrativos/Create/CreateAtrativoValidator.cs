using EcoTurismo.Application.DTOs;
using EcoTurismo.Domain.Enums;
using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Atrativos.Create;

public class CreateAtrativoValidator : Validator<AtrativoCreateRequest>
{
    public CreateAtrativoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("Tipo de atrativo inválido. Use: Balneario, Cachoeira, Trilha, Parque ou FazendaEcoturismo");

        RuleFor(x => x.MunicipioId)
            .NotEmpty().WithMessage("MunicipioId é obrigatório");

        RuleFor(x => x.CapacidadeMaxima)
            .GreaterThan(0).WithMessage("Capacidade máxima deve ser maior que zero");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}
