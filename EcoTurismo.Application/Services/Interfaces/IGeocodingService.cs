using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Application.Services.Interfaces;

public interface IGeocodingService
{
    Task<List<GeocodeResultDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
