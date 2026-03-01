using EcoTurismo.Application.DTOs;

namespace EcoTurismo.Application.Interfaces;
public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(string periodo, CancellationToken ct = default);
}
