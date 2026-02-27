using EcoTurismo.Application.DTOs;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Usuarios;

public class CreateUsuarioValidator : AbstractValidator<UsuarioCreateRequest>
{
    public CreateUsuarioValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(200).WithMessage("Email deve ter no máximo 200 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role é obrigatória");

        RuleFor(x => x.Cpf)
            .MaximumLength(14).WithMessage("CPF deve ter no máximo 14 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Cpf));

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefone));
    }
}
