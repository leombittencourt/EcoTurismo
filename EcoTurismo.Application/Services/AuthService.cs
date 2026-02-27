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
    private readonly IPermissionService _permissionService;

    public AuthService(EcoTurismoDbContext db, IConfiguration config, IPermissionService permissionService)
    {
        _db = db;
        _config = config;
        _permissionService = permissionService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var profile = await _db.Profiles
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.Email == request.Email);

        if (profile is null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, profile.PasswordHash))
            return null;

        var token = await GenerateJwtAsync(profile);

        return new LoginResponse(token, new ProfileDto(
            profile.Id,
            profile.Nome,
            profile.Email,
            profile.Role.Name,
            profile.MunicipioId,
            profile.AtrativoId
        ));
    }

    private async Task<string> GenerateJwtAsync(Profile profile)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, profile.Id.ToString()),
            new Claim(ClaimTypes.Email, profile.Email),
            new Claim(ClaimTypes.Name, profile.Nome),
            new Claim(ClaimTypes.Role, profile.Role.Name),
            new Claim("role_id", profile.RoleId.ToString()),
            new Claim("role_name", profile.Role.Name),
        };

        // Busca permissões do banco de dados
        var permissions = await _permissionService.GetPermissionsByRoleIdAsync(profile.RoleId);
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        // Adiciona informações adicionais se existirem
        if (profile.MunicipioId.HasValue)
        {
            claims.Add(new Claim("municipio_id", profile.MunicipioId.Value.ToString()));
        }

        if (profile.AtrativoId.HasValue)
        {
            claims.Add(new Claim("atrativo_id", profile.AtrativoId.Value.ToString()));
        }

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
