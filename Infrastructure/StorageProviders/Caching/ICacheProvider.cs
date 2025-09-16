using DataService.Models.Entities;

namespace Infrastructure.StorageProviders.Caching
{
    /// <summary>
    /// Defines the contract for a caching provider that handles temporary storage operations
    /// for <see cref="DataEntity"/> objects.
    /// </summary>
    public interface ICacheProvider
    {
        // Future functionality expansion

        /// <summary>
        /// Asynchronously retrieves a <see cref="DataEntity"/> from the cache by its key.
        /// </summary>
        /// <param name="key">The unique key used to identify the cached entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved entity.</returns>
        // Task<DataEntity> GetByIdAsync(string key);

        /// <summary>
        /// Asynchronously writes a <see cref="DataEntity"/> to the cache.
        /// </summary>
        /// <param name="entity">The entity to cache.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the write was successful.</returns>
        // Task<bool> WriteAsync(DataEntity entity);

        /// <summary>
        /// Asynchronously updates an existing <see cref="DataEntity"/> in the cache.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the update was successful.</returns>
        // Task<bool> UpdateAsync(DataEntity entity);
    }
}