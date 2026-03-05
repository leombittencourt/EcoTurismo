using System.Collections.Concurrent;

namespace EcoTurismo.Api.Services;

/// <summary>
/// Serviço para controle de rate limiting por IP/usuário
/// </summary>
public class RateLimitingService
{
    private readonly ConcurrentDictionary<string, RateLimitInfo> _requests = new();
    private readonly TimeSpan _window;
    private readonly int _maxRequests;

    public RateLimitingService(TimeSpan window, int maxRequests)
    {
        _window = window;
        _maxRequests = maxRequests;
        
        // Limpar entradas antigas periodicamente
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                CleanupOldEntries();
            }
        });
    }

    public bool IsAllowed(string key)
    {
        var now = DateTimeOffset.UtcNow;
        
        var info = _requests.GetOrAdd(key, _ => new RateLimitInfo());
        
        lock (info)
        {
            // Remover requisições antigas
            info.Requests.RemoveAll(r => now - r > _window);
            
            if (info.Requests.Count >= _maxRequests)
            {
                return false;
            }
            
            info.Requests.Add(now);
            return true;
        }
    }

    public (int Remaining, DateTimeOffset ResetAt) GetLimitInfo(string key)
    {
        var now = DateTimeOffset.UtcNow;

        if (!_requests.TryGetValue(key, out var info))
        {
            return (_maxRequests, now.Add(_window));
        }

        lock (info)
        {
            info.Requests.RemoveAll(r => now - r > _window);
            var remaining = Math.Max(0, _maxRequests - info.Requests.Count);
            var resetAt = info.Requests.Count > 0 
                ? info.Requests.Min().Add(_window) 
                : now.Add(_window);
            return (remaining, resetAt);
        }
    }

    private void CleanupOldEntries()
    {
        var now = DateTimeOffset.UtcNow;
        var keysToRemove = new List<string>();
        
        foreach (var kvp in _requests)
        {
            lock (kvp.Value)
            {
                kvp.Value.Requests.RemoveAll(r => now - r > _window);
                if (kvp.Value.Requests.Count == 0)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
        }
        
        foreach (var key in keysToRemove)
        {
            _requests.TryRemove(key, out _);
        }
    }

    private class RateLimitInfo
    {
        public List<DateTimeOffset> Requests { get; } = new();
    }
}
