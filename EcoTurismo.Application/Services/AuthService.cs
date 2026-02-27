using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcoTurismo.Application.Services;

public class AuthService : IAuthService
{
    private readonly EcoTurismoDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(EcoTurismoDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var profile = await _db.Profiles
            .FirstOrDefaultAsync(p => p.Email == request.Email);

        if (profile is null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, profile.PasswordHash))
            return null;

        var token = GenerateJwt(profile);

        return new LoginResponse(token, new ProfileDto(
            profile.Id,
            profile.Nome,
            profile.Email,
            profile.Role,
            profile.MunicipioId,
            profile.AtrativoId
        ));
    }

    private string GenerateJwt(Profile profile)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, profile.Id.ToString()),
            new Claim(ClaimTypes.Email, profile.Email),
            new Claim(ClaimTypes.Role, profile.Role),
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
