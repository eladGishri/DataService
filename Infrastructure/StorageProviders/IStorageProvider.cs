using DataService.Models.Entities;
using DataService.Models.Enums;

namespace DataService.Infrastructure.StorageProviders
{
    public interface IStorageProvider
    {
        StorageType Type { get; }
        Task<DataEntity> GetAsync(string id);
        Task<bool> SaveAsync(DataEntity item);
        Task<bool> UpdateAsync(DataEntity item);
        Task<bool> DeleteAsync(string id);
    }
}