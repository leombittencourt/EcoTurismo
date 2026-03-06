using EcoTurismo.Application.DTOs;
using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Usuarios.List;

public class ListUsuariosRequest : PagedRequest
{
    [QueryParam]
    public Guid? MunicipioId { get; set; }
    
    [QueryParam]
    public Guid? RoleId { get; set; }
    
    [QueryParam]
    public bool? Ativo { get; set; }
    
    [QueryParam]
    public string? Search { get; set; }
}
