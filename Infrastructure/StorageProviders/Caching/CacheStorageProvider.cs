using System.Collections.Concurrent;
using System.Runtime.Caching;
using DataService.Infrastructure.StorageProviders;
using DataService.Models.Entities;
using DataService.Models.Enums;

namespace Infrastructure.StorageProviders.Caching
{
    /// <summary>
    /// Provides an in-memory cache implementation of <see cref="IStorageProvider"/> and <see cref="ICacheProvider"/>
    /// for performing temporary CRUD operations on <see cref="DataEntity"/> objects.
    /// </summary>
public class CacheStorageProvider : IStorageProvider, ICacheProvider
{
        /// <summary>
        /// Gets the type of storage used by this provider.
        /// </summary>
    public StorageType Type => StorageType.Cache;

    private const int _cacheExpirationMinutes = 10;
    private static readonly MemoryCache _cache = MemoryCache.Default;

        /// <summary>
        /// Asynchronously retrieves a <see cref="DataEntity"/> from the cache by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The cached <see cref="DataEntity"/>, or <c>null</c> if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
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

        /// <summary>
        /// Asynchronously saves a <see cref="DataEntity"/> to the cache.
        /// </summary>
        /// <param name="item">The entity to cache.</param>
        /// <returns><c>true</c> if the save was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null or has an empty ID.</exception>
    public Task<bool> SaveAsync(DataEntity item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Id))
            throw new ArgumentNullException(nameof(item));

        try
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_cacheExpirationMinutes)
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

        /// <summary>
        /// Asynchronously updates an existing <see cref="DataEntity"/> in the cache.
        /// </summary>
        /// <param name="item">The entity with updated values.</param>
        /// <returns><c>true</c> if the update was successful; otherwise, <c>false</c>.</returns>
    public Task<bool> UpdateAsync(DataEntity item)
    {
        // For in-memory cache, update is equivalent to save (overwrite)
        return SaveAsync(item);
    }

        /// <summary>
        /// Asynchronously deletes a <see cref="DataEntity"/> from the cache by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns><c>true</c> if the deletion was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
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
}