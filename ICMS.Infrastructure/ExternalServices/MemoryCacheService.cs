using ICMS.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace ICMS.Infrastructure.ExternalServices
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly int _defaultTtlMinutes;

        public MemoryCacheService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            if (!int.TryParse(configuration["Cache:DefaultTtlMinutes"], out _defaultTtlMinutes))
            {
                _defaultTtlMinutes = 10;
            }
        }

        public T? Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public void Set<T>(string key, T value, TimeSpan? duration = null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duration ?? TimeSpan.FromMinutes(_defaultTtlMinutes)
            };

            _memoryCache.Set(key, value, cacheEntryOptions);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public bool TryGet<T>(string key, out T? value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }
    }
}
