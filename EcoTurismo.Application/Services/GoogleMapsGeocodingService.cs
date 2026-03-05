using EcoTurismo.Application.DTOs;
using EcoTurismo.Application.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace EcoTurismo.Application.Services;

public class GoogleMapsGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleMapsGeocodingService> _logger;
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, DateTimeOffset> _recentQueries = new();
    private const string GEOCODING_API_URL = "https://maps.googleapis.com/maps/api/geocode/json";
    private const int MAX_RESULTS = 5;
    private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromHours(24);
    private static readonly TimeSpan DUPLICATE_WINDOW = TimeSpan.FromSeconds(10);

    public GoogleMapsGeocodingService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GoogleMapsGeocodingService> logger,
        IMemoryCache cache)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _cache = cache;

        // Limpar queries antigas periodicamente
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                CleanupRecentQueries();
            }
        });
    }

    public async Task<List<GeocodeResultDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        // Normalizar query para cache
        var normalizedQuery = NormalizeQuery(query);
        var cacheKey = $"geocode:{normalizedQuery}";

        // Verificar duplicatas recentes
        if (IsDuplicateQuery(normalizedQuery))
        {
            _logger.LogWarning("Query duplicada detectada em janela curta: {Query}", query);

            // Retornar do cache se existir
            if (_cache.TryGetValue(cacheKey, out List<GeocodeResultDto>? cachedDuplicate) && cachedDuplicate != null)
            {
                return cachedDuplicate;
            }

            throw new InvalidOperationException("Aguarde alguns segundos antes de fazer a mesma busca novamente");
        }

        // Verificar cache
        if (_cache.TryGetValue(cacheKey, out List<GeocodeResultDto>? cachedResult) && cachedResult != null)
        {
            _logger.LogInformation("Retornando resultado do cache para: {Query}", query);
            return cachedResult;
        }

        var apiKey = _configuration["GoogleMaps:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogError("Google Maps API Key não configurada");
            throw new InvalidOperationException("Google Maps API Key não está configurada no appsettings");
        }

        try
        {
            var url = $"{GEOCODING_API_URL}?address={Uri.EscapeDataString(query)}&key={apiKey}&language=pt-BR";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GoogleMapsGeocodeResponse>(cancellationToken);

            if (result == null || result.Results == null || result.Results.Count == 0)
            {
                _logger.LogInformation("Nenhum resultado encontrado para a consulta: {Query}", query);
                return new List<GeocodeResultDto>();
            }

            if (result.Status != "OK")
            {
                _logger.LogWarning("Google Maps API retornou status: {Status}", result.Status);
                return new List<GeocodeResultDto>();
            }

            // Limitar a 5 resultados
            var geocodeResults = result.Results
                .Take(MAX_RESULTS)
                .Select(r => new GeocodeResultDto(
                    DisplayName: r.FormattedAddress ?? "Localização sem nome",
                    Latitude: r.Geometry?.Location?.Lat ?? 0,
                    Longitude: r.Geometry?.Location?.Lng ?? 0,
                    PlaceId: r.PlaceId ?? string.Empty
                )).ToList();

            // Armazenar no cache
            _cache.Set(cacheKey, geocodeResults, CACHE_DURATION);

            // Registrar query recente
            _recentQueries[normalizedQuery] = DateTimeOffset.UtcNow;

            _logger.LogInformation("Buscou {Count} resultados no Google Maps para: {Query}", geocodeResults.Count, query);

            return geocodeResults;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao chamar Google Maps API");
            throw new InvalidOperationException("Erro ao buscar localização no Google Maps", ex);
        }
    }

    private string NormalizeQuery(string query)
    {
        return query.Trim().ToLowerInvariant()
            .Replace("  ", " ") // Remover espaços duplos
            .Replace(",", "") // Remover vírgulas
            .Replace(".", ""); // Remover pontos
    }

    private bool IsDuplicateQuery(string normalizedQuery)
    {
        if (_recentQueries.TryGetValue(normalizedQuery, out var lastQuery))
        {
            return DateTimeOffset.UtcNow - lastQuery < DUPLICATE_WINDOW;
        }
        return false;
    }

    private void CleanupRecentQueries()
    {
        var now = DateTimeOffset.UtcNow;
        var keysToRemove = _recentQueries
            .Where(kvp => now - kvp.Value > DUPLICATE_WINDOW)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _recentQueries.TryRemove(key, out _);
        }
    }

    // Classes para deserialização da resposta do Google Maps
    private class GoogleMapsGeocodeResponse
    {
        [JsonPropertyName("results")]
        public List<GoogleMapsResult> Results { get; set; } = new();

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    private class GoogleMapsResult
    {
        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }

        [JsonPropertyName("geometry")]
        public GoogleMapsGeometry? Geometry { get; set; }

        [JsonPropertyName("place_id")]
        public string? PlaceId { get; set; }
    }

    private class GoogleMapsGeometry
    {
        [JsonPropertyName("location")]
        public GoogleMapsLocation? Location { get; set; }
    }

    private class GoogleMapsLocation
    {
        [JsonPropertyName("lat")]
        public decimal Lat { get; set; }

        [JsonPropertyName("lng")]
        public decimal Lng { get; set; }
    }
}
