using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Usuarios;

public class UpdateUsuarioValidator : AbstractValidator<UpdateUsuarioRequest>
{
    public UpdateUsuarioValidator()
    {
        RuleFor(x => x.Nome)
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Nome));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(200).WithMessage("Email deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Password));

        RuleFor(x => x.Cpf)
            .MaximumLength(14).WithMessage("CPF deve ter no máximo 14 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Cpf));

        RuleFor(x => x.Telefone)
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefone));
    }
}
