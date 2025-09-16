using DataService.Models.Entities;
using DataService.Infrastructure.StorageProviders;
using DataService.Models.Enums;
using Microsoft.EntityFrameworkCore;
using DataService.Infrastructure;

namespace Infrastructure.StorageProviders.Repositories
{
    /// <summary>
    /// Provides database-backed implementation of <see cref="IStorageProvider"/> and <see cref="IDataRepository"/>
    /// for performing CRUD operations on <see cref="DataEntity"/> using Entity Framework.
    /// </summary>
    public class DatabaseStorageProvider : IStorageProvider, IDataRepository
    {
        /// <summary>
        /// Gets the type of storage used by this provider.
        /// </summary>
        public StorageType Type => StorageType.Database;

        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseStorageProvider"/> class.
        /// </summary>
        /// <param name="context">The database context used for data access.</param>
        public DatabaseStorageProvider(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously retrieves a <see cref="DataEntity"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>The matching <see cref="DataEntity"/>, or <c>null</c> if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
        public async Task<DataEntity> GetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            return await _context.DataEntities.FindAsync(id);
        }

        /// <summary>
        /// Asynchronously saves a new <see cref="DataEntity"/> to the database.
        /// </summary>
        /// <param name="item">The entity to save.</param>
        /// <returns><c>true</c> if the save was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null or has an empty ID.</exception>
        /// <exception cref="Exception">Thrown when the save operation fails.</exception>
        public async Task<bool> SaveAsync(DataEntity item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
                throw new ArgumentNullException(nameof(item));

            try
            {
                _context.DataEntities.Add(item);
                return (await _context.SaveChangesAsync() > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save data entity to database.", ex);
            }
        }

        /// <summary>
        /// Asynchronously updates an existing <see cref="DataEntity"/> in the database.
        /// </summary>
        /// <param name="item">The entity with updated values.</param>
        /// <returns><c>true</c> if the update was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null or has an empty ID.</exception>
        /// <exception cref="Exception">Thrown when the update operation fails.</exception>
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
                return (await _context.SaveChangesAsync() > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update data entity in database.", ex);
            }
        }

        /// <summary>
        /// Asynchronously deletes a <see cref="DataEntity"/> from the database by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns><c>true</c> if the deletion was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
        /// <exception cref="Exception">Thrown when the delete operation fails.</exception>
        public async Task<bool> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            try
            {
                var entity = await _context.DataEntities.FindAsync(id);
                if (entity == null) return false;

                _context.DataEntities.Remove(entity);
                return (await _context.SaveChangesAsync() > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete data entity from database.", ex);
            }
        }
    }
}