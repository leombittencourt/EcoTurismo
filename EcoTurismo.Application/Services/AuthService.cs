using EcoTurismo.Application.Auth;
using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcoTurismo.Application.Services;

public class AuthService : IAuthService
{
    private readonly EcoTurismoDbContext _db;
    private readonly JwtSettings _jwtSettings;
    private readonly IPermissionService _permissionService;

    public AuthService(EcoTurismoDbContext db, IOptions<JwtSettings> jwtSettings, IPermissionService permissionService)
    {
        _db = db;
        _jwtSettings = jwtSettings.Value;
        _permissionService = permissionService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            // Buscar usuário por email (case insensitive)
            var usuario = await _db.Usuarios
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (usuario is null)
            {
                Console.WriteLine($"❌ Login falhou: Usuário não encontrado com email '{request.Email}'");
                return null;
            }

            // Verificar se usuário está ativo
            if (!usuario.Ativo)
            {
                Console.WriteLine($"❌ Login falhou: Usuário '{usuario.Email}' está inativo");
                return null;
            }

            // Verificar senha
            var senhaValida = BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash);
            if (!senhaValida)
            {
                Console.WriteLine($"❌ Login falhou: Senha incorreta para usuário '{usuario.Email}'");
                return null;
            }

            // Gerar token JWT
            var token = await GenerateJwtAsync(usuario);

            Console.WriteLine($"✅ Login bem-sucedido: {usuario.Email} ({usuario.Role.Name})");

            return new LoginResponse(token, new ProfileDto(
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.Role.Name,
                usuario.MunicipioId,
                usuario.AtrativoId
            ));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro no login: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            throw;
        }
    }

    private async Task<string> GenerateJwtAsync(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Role.Name),
            new Claim("role_id", usuario.RoleId.ToString()),
            new Claim("role_name", usuario.Role.Name),
        };

        // Busca permissões do banco de dados
        var permissions = await _permissionService.GetPermissionsByRoleIdAsync(usuario.RoleId);
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        // Adiciona informações adicionais se existirem
        if (usuario.MunicipioId.HasValue)
        {
            claims.Add(new Claim("municipio_id", usuario.MunicipioId.Value.ToString()));
        }

        if (usuario.AtrativoId.HasValue)
        {
            claims.Add(new Claim("atrativo_id", usuario.AtrativoId.Value.ToString()));
        }

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
