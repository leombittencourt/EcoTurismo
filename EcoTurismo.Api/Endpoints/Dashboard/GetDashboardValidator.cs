using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Dashboard;

public class GetDashboardValidator : Validator<GetDashboardRequest>
{
    private static readonly string[] PeriodosValidos = { "7d", "30d", "6m" };

    public GetDashboardValidator()
    {
        RuleFor(x => x.Periodo)
            .NotEmpty()
            .WithMessage("O período é obrigatório.")
            .Must(p => PeriodosValidos.Contains(p))
            .WithMessage("Período inválido. Use: 7d, 30d ou 6m.");
    }
}
