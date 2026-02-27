using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
