using DataService.Infrastructure.StorageProviders;
using DataService.Models.Enums;

namespace DataService.Infrastructure.StorageProviders
{
    /// <summary>
    /// Factory class responsible for retrieving the appropriate <see cref="IStorageProvider"/>
    /// implementation based on the specified <see cref="StorageType"/>.
    /// </summary>
    public class StorageProviderFactory : IStorageProviderFactory
    {
        private readonly IEnumerable<IStorageProvider> _providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageProviderFactory"/> class.
        /// </summary>
        /// <param name="providers">A collection of available <see cref="IStorageProvider"/> implementations.</param>
        public StorageProviderFactory(IEnumerable<IStorageProvider> providers)
        {
            _providers = providers;
        }

        /// <summary>
        /// Retrieves the <see cref="IStorageProvider"/> that matches the specified <see cref="StorageType"/>.
        /// </summary>
        /// <param name="type">The type of storage provider to retrieve.</param>
        /// <returns>The matching <see cref="IStorageProvider"/> instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no provider matches the specified <see cref="StorageType"/>.
        /// </exception>
        public IStorageProvider GetStorageProvider(StorageType type)
        {
            return _providers.First(p => p.Type == type);
        }
    }
}
