using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Api.Endpoints.Municipios;

public class ListMunicipiosResponse
{
    public List<MunicipioDto> Municipios { get; set; } = [];
}
