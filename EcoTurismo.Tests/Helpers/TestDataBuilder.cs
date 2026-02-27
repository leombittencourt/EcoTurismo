using EcoTurismo.Domain.Entities;

namespace EcoTurismo.Tests.Helpers;

public static class TestDataBuilder
{
    public static Role CreateRole(string name = "Admin")
    {
        return new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            NormalizedName = name.ToUpperInvariant(),
            Description = $"Test {name} role",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public static Usuario CreateUsuario(
        string nome = "Test User",
        string email = "test@example.com",
        Guid? roleId = null,
        string? telefone = null,
        string? cpf = null)
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test123"),
            RoleId = roleId ?? Guid.NewGuid(),
            Telefone = telefone,
            Cpf = cpf,
            Ativo = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public static Municipio CreateMunicipio(string nome = "Test City", string uf = "MS")
    {
        return new Municipio
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Uf = uf,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
