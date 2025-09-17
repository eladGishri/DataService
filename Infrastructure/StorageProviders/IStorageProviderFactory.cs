using DataService.Infrastructure.StorageProviders;
using DataService.Models.Entities;

namespace DataService.Infrastructure.StorageProviders
{
    /// <summary>
    /// Factory interface for resolving <see cref="IStorageProvider"/> instances.
    /// Responsible for providing access to storage providers
    /// (e.g., cache, file, database) in the configured priority order.
    /// </summary>
    public interface IStorageProviderFactory
    {
        /// <summary>
        /// Gets all registered <see cref="IStorageProvider"/> instances 
        /// in the configured priority order.
        /// </summary>
        /// <returns>
        /// An ordered sequence of storage providers, 
        /// starting from fastest/cheapest (e.g., cache) 
        /// to most persistent (e.g., database).
        /// </returns>
        IEnumerable<IStorageProvider> GetAll();

        /// <summary>
        /// Gets all <see cref="IStorageProvider"/> instances 
        /// that are positioned before the specified provider in the chain.
        /// <br/>
        /// This is mainly used to refresh higher-level providers (cache/file) 
        /// after a value is retrieved from a lower-level provider (e.g., database).
        /// </summary>
        /// <param name="provider">
        /// The provider that successfully retrieved the data.
        /// </param>
        /// <returns>
        /// A sequence of providers that appear earlier in the order 
        /// than the given provider.
        /// </returns>
        IEnumerable<IStorageProvider> GetProvidersBefore(IStorageProvider provider);
    }
}
