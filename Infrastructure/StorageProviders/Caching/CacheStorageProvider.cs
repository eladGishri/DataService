using System.Collections.Concurrent;
using System.Runtime.Caching;
using DataService.Infrastructure.StorageProviders;
using DataService.Models.Entities;
using DataService.Models.Enums;

namespace Infrastructure.StorageProviders.Caching;

public class CacheStorageProvider : IStorageProvider, ICacheProvider
{
    public StorageType Type => StorageType.Cache;
    private const int _cacheExpirationMinutes = 10;
    private static readonly MemoryCache _cache = MemoryCache.Default;

    public Task<DataEntity> GetAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentNullException(nameof(id));

        try
        {
            var entity = _cache.Get(id) as DataEntity;
            if (entity != null)
            {
                return Task.FromResult(entity);
            }
            return Task.FromResult<DataEntity>(null);
        }
        catch (Exception ex)
        {
            // Todo: Log exception
            return Task.FromResult<DataEntity>(null);
        }
    }

    public Task<bool> SaveAsync(DataEntity item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Id))
            throw new ArgumentNullException(nameof(item));

        try
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(_cacheExpirationMinutes)
            };
            _cache.Set(item.Id, item, policy);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            // Todo: Log exception
            return Task.FromResult(false);
        }
        
    }

    public Task<bool> UpdateAsync(DataEntity item)
    {
        // For in-memory cache, update is equivalent to save (overwrite)
        return SaveAsync(item);
    }

    public Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentNullException(nameof(id));

        try
        {
            _cache.Remove(id);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            // Todo: Log exception
            return Task.FromResult(false);
        }
    }
}
