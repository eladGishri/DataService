using DataService.Models.Entities;
using DataService.Infrastructure.StorageProviders;
using DataService.Models.Enums;
using Microsoft.EntityFrameworkCore;
using DataService.Infrastructure;

namespace Infrastructure.StorageProviders.Repositories
{
    public class DatabaseStorageProvider : IStorageProvider, IDataRepository
    {
        public StorageType Type => StorageType.Database;
        private readonly ApplicationDbContext _context;

        public DatabaseStorageProvider(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DataEntity> GetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            return await _context.DataEntities.FindAsync(id);
        }

        public async Task<bool> SaveAsync(DataEntity item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
                throw new ArgumentNullException(nameof(item));

            try
            {
                _context.DataEntities.Add(item);
                return (await _context.SaveChangesAsync() > 0) ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save data entity to database.", ex);
            }
        }

        public async Task<bool> UpdateAsync(DataEntity item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
                throw new ArgumentNullException(nameof(item));

            try
            {
                var existing = await _context.DataEntities.FindAsync(item.Id);
                if (existing == null) return false;

                existing.Value = item.Value;
                existing.CreatedAt = item.CreatedAt;
                return (await _context.SaveChangesAsync() > 0) ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update data entity in database.", ex);
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            try
            {
                var entity = await _context.DataEntities.FindAsync(id);
                if (entity == null) return false;

                _context.DataEntities.Remove(entity);
                return (await _context.SaveChangesAsync() > 0) ? true : false;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete data entity from database.", ex);
            }
        }
    }
}