using DataService.Models.Entities;

namespace Infrastructure.StorageProviders.FileStorage
{
    /// <summary>
    /// Defines the contract for a file-based storage provider that handles persistence operations
    /// for <see cref="DataEntity"/> objects.
    /// </summary>
    public interface IFileProvider
    {
        // Future functionality expansion

        /// <summary>
        /// Asynchronously reads a <see cref="DataEntity"/> from a file by its unique identifier.
        /// </summary>
        /// <typeparam name="T">The type of the entity to read.</typeparam>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved entity.</returns>
        // Task<DataEntity> ReadAsync<T>(string id);

        /// <summary>
        /// Asynchronously writes a <see cref="DataEntity"/> to a file.
        /// </summary>
        /// <param name="entity">The entity to write.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the write was successful.</returns>
        // Task<bool> WriteAsync(DataEntity entity);

        /// <summary>
        /// Asynchronously updates an existing <see cref="DataEntity"/> in a file.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the update was successful.</returns>
        // Task<bool> UpdateAsync(DataEntity entity);
    }
}