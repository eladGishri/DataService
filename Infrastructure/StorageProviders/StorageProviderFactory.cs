using DataService.Infrastructure.StorageProviders;
using DataService.Models.Enums;

namespace DataService.Infrastructure.StorageProviders
{
    public class StorageProviderFactory : IStorageProviderFactory
    {
        private readonly IEnumerable<IStorageProvider> _providers;

        public StorageProviderFactory(IEnumerable<IStorageProvider> providers)
        {
            _providers = providers;
        }

        public IStorageProvider GetStorageProvider(StorageType type)
        {
            return _providers.First(p => p.Type == type);
        }
    }
}
