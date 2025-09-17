using DataService.Infrastructure.StorageProviders;
using Infrastructure.StorageProviders.Caching;
using Infrastructure.StorageProviders.FileStorage;
using Infrastructure.StorageProviders.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataService.Infrastructure.Factories
{
    /// <summary>
    /// Factory responsible for managing and providing access to storage providers
    /// in a predefined priority order (e.g., Cache → File → Database).
    /// </summary>
    public class StorageProviderFactory : IStorageProviderFactory
    {
        private readonly List<IStorageProvider> _providers;

        /// <summary>
        /// Initializes the factory with the given providers.
        /// The order defines their priority (first = highest priority).
        /// </summary>
        /// <param name="providers">A list of storage providers.</param>
        /// <exception cref="ArgumentNullException">Thrown if providers list is null or empty.</exception>
        public StorageProviderFactory(IEnumerable<IStorageProvider> providers)
        {
            if (providers == null || !providers.Any())
            {
                throw new ArgumentException("At least one storage provider must be registered.", nameof(providers));
            }

            // Enforce a specific priority order (Cache → File → Database)
            _providers = providers.OrderBy(p =>
            {
                return p switch
                {
                    CacheStorageProvider => 0,
                    FileStorageProvider => 1,
                    DatabaseStorageProvider => 2,
                    _ => 99
                };
            }).ToList();
        }

        /// <summary>
        /// Gets all providers in the configured priority order.
        /// </summary>
        /// <returns>An ordered enumerable of storage providers.</returns>
        public IEnumerable<IStorageProvider> GetAll()
        {
            if (_providers == null || !_providers.Any())
            {
                throw new InvalidOperationException("No storage providers have been configured.");
            }

            return _providers;
        }

        /// <summary>
        /// Returns all providers that appear before the specified provider in the priority chain.
        /// This is useful when refreshing higher-level providers with data retrieved
        /// from a lower-level provider (e.g., refreshing cache after reading from DB).
        /// </summary>
        /// <param name="provider">The provider to look up.</param>
        /// <returns>A sequence of providers that come before the given provider.</returns>
        /// <exception cref="ArgumentNullException">Thrown if provider is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the provider is not registered in the factory.</exception>
        public IEnumerable<IStorageProvider> GetProvidersBefore(IStorageProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            var index = _providers.IndexOf(provider);
            if (index == -1)
            {
                throw new InvalidOperationException(
                    $"The specified provider '{provider.GetType().Name}' is not registered in this factory.");
            }

            if (index == 0)
            {
                // No providers before the first one
                return Enumerable.Empty<IStorageProvider>();
            }

            return _providers.Take(index);
        }
    }
}
