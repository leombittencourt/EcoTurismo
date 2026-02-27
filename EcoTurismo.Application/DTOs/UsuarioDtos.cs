namespace EcoTurismo.Application.DTOs;

public record UsuarioDto(
    Guid Id,
    string Nome,
    string Email,
    string RoleName,
    Guid RoleId,
    Guid? MunicipioId,
    Guid? AtrativoId,
    string? Telefone,
    string? Cpf,
    bool Ativo
);

public record UsuarioCreateRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public Guid RoleId { get; init; }
    public Guid? MunicipioId { get; init; }
    public Guid? AtrativoId { get; init; }
    public string? Telefone { get; init; }
    public string? Cpf { get; init; }
}

public record UsuarioUpdateRequest
{
    public string? Nome { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
    public Guid? RoleId { get; init; }
    public Guid? MunicipioId { get; init; }
    public Guid? AtrativoId { get; init; }
    public string? Telefone { get; init; }
    public string? Cpf { get; init; }
    public bool? Ativo { get; init; }
}

public record UsuarioListItem(
    Guid Id,
    string Nome,
    string Email,
    string RoleName,
    bool Ativo
);
