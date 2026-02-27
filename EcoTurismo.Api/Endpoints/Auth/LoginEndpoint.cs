using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using FastEndpoints;
using FluentValidation;

namespace EcoTurismo.Api.Endpoints.Auth;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public ProfileDto Profile { get; set; } = null!;
}

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória");
    }
}

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IAuthService _auth;

    public LoginEndpoint(IAuthService auth) => _auth = auth;

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _auth.LoginAsync(new Application.DTOs.LoginRequest(req.Email, req.Password));

        if (result is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        await Send.OkAsync(new LoginResponse
        {
            Token = result.Token,
            Profile = result.Profile
        }, ct);
    }
}
