using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Interfaces;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly EcoTurismoDbContext _db;

    public UsuarioService(EcoTurismoDbContext db)
    {
        _db = db;
    }

    public async Task<List<UsuarioListItem>> ListarAsync()
    {
        return await _db.Usuarios
            .Include(u => u.Role)
            .Where(u => u.Ativo)
            .OrderBy(u => u.Nome)
            .Select(u => new UsuarioListItem(
                u.Id,
                u.Nome,
                u.Email,
                u.Role.Name,
                u.Ativo
            ))
            .ToListAsync();
    }

    public async Task<UsuarioDto?> ObterPorIdAsync(Guid id)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario is null)
            return null;

        return new UsuarioDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Role.Name,
            usuario.RoleId,
            usuario.MunicipioId,
            usuario.AtrativoId,
            usuario.Telefone,
            usuario.Cpf,
            usuario.Ativo
        );
    }

    public async Task<UsuarioDto> CriarAsync(UsuarioCreateRequest request)
    {
        // Verifica se já existe usuário com esse email
        var existente = await _db.Usuarios
            .AnyAsync(u => u.Email == request.Email);

        if (existente)
            throw new InvalidOperationException("Já existe um usuário com este email.");

        // Verifica se a role existe
        var role = await _db.Roles
            .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.IsActive);

        if (role is null)
            throw new InvalidOperationException("Role não encontrada ou inativa.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = request.RoleId,
            MunicipioId = request.MunicipioId,
            AtrativoId = request.AtrativoId,
            Telefone = request.Telefone,
            Cpf = request.Cpf,
            Ativo = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return new UsuarioDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            role.Name,
            usuario.RoleId,
            usuario.MunicipioId,
            usuario.AtrativoId,
            usuario.Telefone,
            usuario.Cpf,
            usuario.Ativo
        );
    }

    public async Task<UsuarioDto?> AtualizarAsync(Guid id, UsuarioUpdateRequest request)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario is null)
            return null;

        // Verifica se o novo email já está em uso por outro usuário
        if (request.Email != null && request.Email != usuario.Email)
        {
            var emailEmUso = await _db.Usuarios
                .AnyAsync(u => u.Email == request.Email && u.Id != id);

            if (emailEmUso)
                throw new InvalidOperationException("Este email já está em uso por outro usuário.");
        }

        // Atualiza campos se fornecidos
        if (request.Nome != null)
            usuario.Nome = request.Nome;

        if (request.Email != null)
            usuario.Email = request.Email;

        if (request.Password != null)
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        if (request.RoleId.HasValue)
        {
            var role = await _db.Roles.FindAsync(request.RoleId.Value);
            if (role is null || !role.IsActive)
                throw new InvalidOperationException("Role não encontrada ou inativa.");
            
            usuario.RoleId = request.RoleId.Value;
        }

        if (request.MunicipioId.HasValue)
            usuario.MunicipioId = request.MunicipioId.Value;

        if (request.AtrativoId.HasValue)
            usuario.AtrativoId = request.AtrativoId.Value;

        if (request.Telefone != null)
            usuario.Telefone = request.Telefone;

        if (request.Cpf != null)
            usuario.Cpf = request.Cpf;

        if (request.Ativo.HasValue)
            usuario.Ativo = request.Ativo.Value;

        usuario.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();

        // Recarrega a role se foi alterada
        await _db.Entry(usuario).Reference(u => u.Role).LoadAsync();

        return new UsuarioDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Role.Name,
            usuario.RoleId,
            usuario.MunicipioId,
            usuario.AtrativoId,
            usuario.Telefone,
            usuario.Cpf,
            usuario.Ativo
        );
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);

        if (usuario is null)
            return false;

        _db.Usuarios.Remove(usuario);
        await _db.SaveChangesAsync();

        return true;
    }
}
