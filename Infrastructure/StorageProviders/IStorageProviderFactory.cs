using DataService.Models.Enums;

namespace DataService.Infrastructure.StorageProviders
{
    public interface IStorageProviderFactory
    {
        IStorageProvider GetStorageProvider(StorageType type);
    }
}
