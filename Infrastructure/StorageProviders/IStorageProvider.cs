using DataService.Models.Entities;
using DataService.Models.Enums;

namespace DataService.Infrastructure.StorageProviders
{
    /// <summary>
    /// Defines the contract for a storage provider that handles basic CRUD operations
    /// for <see cref="DataEntity"/> objects using a specific <see cref="StorageType"/>.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Gets the type of storage this provider implements.
        /// </summary>
        StorageType Type { get; }

        /// <summary>
        /// Asynchronously retrieves a <see cref="DataEntity"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved <see cref="DataEntity"/>.</returns>
        Task<DataEntity> GetAsync(string id);

        /// <summary>
        /// Asynchronously saves a new <see cref="DataEntity"/> to the storage.
        /// </summary>
        /// <param name="item">The entity to save.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the save was successful.</returns>
        Task<bool> SaveAsync(DataEntity item);

        /// <summary>
        /// Asynchronously updates an existing <see cref="DataEntity"/> in the storage.
        /// </summary>
        /// <param name="item">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the update was successful.</returns>
        Task<bool> UpdateAsync(DataEntity item);

        /// <summary>
        /// Asynchronously deletes a <see cref="DataEntity"/> from the storage by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the deletion was successful.</returns>
        Task<bool> DeleteAsync(string id);
    }
}