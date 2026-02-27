using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Api.Endpoints.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public ProfileDto Profile { get; set; } = null!;
}
