using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Auth;

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .WithMessage("Email inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(3).WithMessage("Senha deve ter no mínimo 3 caracteres");
    }
}
