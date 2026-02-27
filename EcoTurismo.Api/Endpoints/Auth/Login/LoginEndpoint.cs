using EcoTurismo.Application.Interfaces;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Auth;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IAuthService _authService;

    public LoginEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        // Validar entrada
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        // Chamar serviço de autenticação
        var loginRequest = new Application.DTOs.LoginRequest(req.Email, req.Password);
        var result = await _authService.LoginAsync(loginRequest);

        if (result is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        // Retornar resposta de sucesso
        await Send.OkAsync(new LoginResponse
        {
            Token = result.Token,
            Profile = result.Profile
        }, ct);
    }
}
