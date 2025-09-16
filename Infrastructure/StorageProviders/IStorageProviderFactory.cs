using DataService.Models.Enums;

namespace DataService.Infrastructure.StorageProviders
{
    /// <summary>
    /// Defines a factory interface for retrieving <see cref="IStorageProvider"/> instances
    /// based on a specified <see cref="StorageType"/>.
    /// </summary>
    public interface IStorageProviderFactory
    {
        /// <summary>
        /// Retrieves the <see cref="IStorageProvider"/> that corresponds to the given <see cref="StorageType"/>.
        /// </summary>
        /// <param name="type">The type of storage provider to retrieve.</param>
        /// <returns>The matching <see cref="IStorageProvider"/> implementation.</returns>
        IStorageProvider GetStorageProvider(StorageType type);
    }
}