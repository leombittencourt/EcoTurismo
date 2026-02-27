using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Services;
using EcoTurismo.Domain.Entities;
using EcoTurismo.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Tests.Services;

public class UsuarioServiceTests
{
    [Fact]
    public async Task CriarAsync_DeveCrearUsuarioComSucesso()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole("Admin");
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();

        var service = new UsuarioService(context);
        var request = new UsuarioCreateRequest
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Password = "senha123",
            RoleId = role.Id,
            Telefone = "(11) 98765-4321"
        };

        // Act
        var result = await service.CriarAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be("João Silva");
        result.Email.Should().Be("joao@example.com");
        result.RoleName.Should().Be("Admin");
        result.Ativo.Should().BeTrue();

        var usuarioNoBanco = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "joao@example.com");
        usuarioNoBanco.Should().NotBeNull();
        BCrypt.Net.BCrypt.Verify("senha123", usuarioNoBanco!.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecaoSeEmailJaExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var usuarioExistente = TestDataBuilder.CreateUsuario(
            email: "existente@example.com",
            roleId: role.Id);
        await context.Usuarios.AddAsync(usuarioExistente);
        await context.SaveChangesAsync();

        var service = new UsuarioService(context);
        var request = new UsuarioCreateRequest
        {
            Nome = "Novo Usuario",
            Email = "existente@example.com",
            Password = "senha123",
            RoleId = role.Id
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CriarAsync(request));
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecaoSeRoleNaoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var service = new UsuarioService(context);
        var request = new UsuarioCreateRequest
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Password = "senha123",
            RoleId = Guid.NewGuid() // Role que não existe
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CriarAsync(request));
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarApenasUsuariosAtivos()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var usuario1 = TestDataBuilder.CreateUsuario(
            nome: "Usuario Ativo 1",
            email: "ativo1@example.com",
            roleId: role.Id);
        usuario1.Ativo = true;

        var usuario2 = TestDataBuilder.CreateUsuario(
            nome: "Usuario Ativo 2",
            email: "ativo2@example.com",
            roleId: role.Id);
        usuario2.Ativo = true;

        var usuario3 = TestDataBuilder.CreateUsuario(
            nome: "Usuario Inativo",
            email: "inativo@example.com",
            roleId: role.Id);
        usuario3.Ativo = false;

        await context.Usuarios.AddRangeAsync(usuario1, usuario2, usuario3);
        await context.SaveChangesAsync();

        var service = new UsuarioService(context);

        // Act
        var result = await service.ListarAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(u => u.Ativo.Should().BeTrue());
        result.Should().Contain(u => u.Nome == "Usuario Ativo 1");
        result.Should().Contain(u => u.Nome == "Usuario Ativo 2");
        result.Should().NotContain(u => u.Nome == "Usuario Inativo");
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarUsuarioQuandoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole("Prefeitura");
        await context.Roles.AddAsync(role);

        var usuario = TestDataBuilder.CreateUsuario(
            nome: "Maria Santos",
            email: "maria@example.com",
            roleId: role.Id,
            telefone: "(67) 3000-0001");

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var service = new UsuarioService(context);

        // Act
        var result = await service.ObterPorIdAsync(usuario.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(usuario.Id);
        result.Nome.Should().Be("Maria Santos");
        result.Email.Should().Be("maria@example.com");
        result.RoleName.Should().Be("Prefeitura");
        result.Telefone.Should().Be("(67) 3000-0001");
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNullQuandoNaoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var service = new UsuarioService(context);

        // Act
        var result = await service.ObterPorIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarCamposFornecidos()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var usuario = TestDataBuilder.CreateUsuario(
            nome: "Nome Original",
            email: "original@example.com",
            roleId: role.Id);

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var service = new UsuarioService(context);
        var request = new UsuarioUpdateRequest
        {
            Nome = "Nome Atualizado",
            Telefone = "(11) 99999-9999"
        };

        // Act
        var result = await service.AtualizarAsync(usuario.Id, request);

        // Assert
        result.Should().NotBeNull();
        result!.Nome.Should().Be("Nome Atualizado");
        result.Email.Should().Be("original@example.com"); // Não foi alterado
        result.Telefone.Should().Be("(11) 99999-9999");
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarSenhaSeForFornecida()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var usuario = TestDataBuilder.CreateUsuario(roleId: role.Id);
        var senhaOriginal = usuario.PasswordHash;

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var service = new UsuarioService(context);
        var request = new UsuarioUpdateRequest
        {
            Password = "novaSenha123"
        };

        // Act
        await service.AtualizarAsync(usuario.Id, request);

        // Assert
        var usuarioAtualizado = await context.Usuarios.FindAsync(usuario.Id);
        usuarioAtualizado!.PasswordHash.Should().NotBe(senhaOriginal);
        BCrypt.Net.BCrypt.Verify("novaSenha123", usuarioAtualizado.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecaoSeEmailJaExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var usuario1 = TestDataBuilder.CreateUsuario(
            email: "usuario1@example.com",
            roleId: role.Id);
        var usuario2 = TestDataBuilder.CreateUsuario(
            email: "usuario2@example.com",
            roleId: role.Id);

        await context.Usuarios.AddRangeAsync(usuario1, usuario2);
        await context.SaveChangesAsync();

        var service = new UsuarioService(context);
        var request = new UsuarioUpdateRequest
        {
            Email = "usuario2@example.com" // Email já usado por usuario2
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.AtualizarAsync(usuario1.Id, request));
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarNullSeUsuarioNaoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var service = new UsuarioService(context);
        var request = new UsuarioUpdateRequest { Nome = "Teste" };

        // Act
        var result = await service.AtualizarAsync(Guid.NewGuid(), request);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExcluirAsync_DeveExcluirUsuarioQuandoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var role = TestDataBuilder.CreateRole();
        await context.Roles.AddAsync(role);

        var usuario = TestDataBuilder.CreateUsuario(roleId: role.Id);
        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        var service = new UsuarioService(context);

        // Act
        var result = await service.ExcluirAsync(usuario.Id);

        // Assert
        result.Should().BeTrue();

        var usuarioNoBanco = await context.Usuarios.FindAsync(usuario.Id);
        usuarioNoBanco.Should().BeNull();
    }

    [Fact]
    public async Task ExcluirAsync_DeveRetornarFalseQuandoUsuarioNaoExiste()
    {
        // Arrange
        using var context = DatabaseHelper.CreateInMemoryContext();
        var service = new UsuarioService(context);

        // Act
        var result = await service.ExcluirAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }
}
