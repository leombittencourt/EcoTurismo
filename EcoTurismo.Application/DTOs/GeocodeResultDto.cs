namespace EcoTurismo.Application.DTOs;

public record GeocodeResultDto(
    string DisplayName,
    decimal Latitude,
    decimal Longitude,
    string PlaceId
);
