using System.Text.Json;
using DataService.Models.Entities;
using DataService.Infrastructure.StorageProviders;
using DataService.Models.Enums;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.StorageProviders.FileStorage
{
    /// <summary>
    /// Provides a file-based implementation of <see cref="IStorageProvider"/> and <see cref="IFileProvider"/>
    /// for performing CRUD operations on <see cref="DataEntity"/> objects.
    /// </summary>
    public class FileStorageProvider : IStorageProvider, IFileProvider
    {
        /// <summary>
        /// Gets the type of storage used by this provider.
        /// </summary>
        public StorageType Type => StorageType.File;

        private const int _fileExpirationMinutes = 30;
        private readonly string _storageDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageProvider"/> class using configuration settings.
        /// </summary>
        /// <param name="configuration">The application configuration containing file storage settings.</param>
        /// <exception cref="Exception">Thrown when initialization fails.</exception>
        public FileStorageProvider(IConfiguration configuration)
        {
            try
            {
                _storageDirectory = configuration["FileStorage:BaseDirectory"];
                if (string.IsNullOrWhiteSpace(_storageDirectory))
                {
                    _storageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "FileStorage");
                }

                if (!Directory.Exists(_storageDirectory))
                    Directory.CreateDirectory(_storageDirectory);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize file storage provider.", ex);
            }
        }

        /// <summary>
        /// Constructs the file path for a given entity ID and optional expiration timestamp.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <param name="expiration">Optional expiration timestamp.</param>
        /// <returns>The full file path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
        /// <exception cref="Exception">Thrown when file path resolution fails.</exception>
        private string GetFilePath(string id, DateTime? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            try
            {
                if (expiration == null)
                {
                    var files = Directory.GetFiles(_storageDirectory, $"{id}_*.json");
                    return files.FirstOrDefault() ?? string.Empty;
                }
                else
                {
                    return Path.Combine(_storageDirectory, $"{id}_{expiration:yyyyMMddHHmmss}.json");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get file path.", ex);
            }
        }

        /// <summary>
        /// Asynchronously retrieves a <see cref="DataEntity"/> from file storage by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The retrieved <see cref="DataEntity"/>, or <c>null</c> if not found or expired.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
        /// <exception cref="Exception">Thrown when retrieval fails.</exception>
        public async Task<DataEntity> GetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            try
            {
                var filePath = GetFilePath(id);
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    return null;

                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var parts = fileName.Split('_');
                if (parts.Length != 2 || !DateTime.TryParseExact(parts[1], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out var expiration))
                    return null;

                if (DateTime.Now > expiration)
                {
                    File.Delete(filePath);
                    return null;
                }

                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<DataEntity>(json);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get data entity from file storage.", ex);
            }
        }

        /// <summary>
        /// Asynchronously saves a <see cref="DataEntity"/> to file storage.
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
                var oldFile = GetFilePath(item.Id);
                if (!string.IsNullOrEmpty(oldFile) && File.Exists(oldFile))
                {
                    File.Delete(oldFile);
                }

                var expiration = DateTime.Now.AddMinutes(_fileExpirationMinutes);
                var filePath = GetFilePath(item.Id, expiration);
                var json = JsonSerializer.Serialize(item);
                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save data entity to file storage.", ex);
            }
        }

        /// <summary>
        /// Asynchronously updates an existing <see cref="DataEntity"/> in file storage.
        /// </summary>
        /// <param name="item">The entity with updated values.</param>
        /// <returns><c>true</c> if the update was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null or has an empty ID.</exception>
        public Task<bool> UpdateAsync(DataEntity item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
                throw new ArgumentNullException(nameof(item));

            return SaveAsync(item);
        }

        /// <summary>
        /// Asynchronously deletes a <see cref="DataEntity"/> from file storage by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns><c>true</c> if the deletion was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
        /// <exception cref="Exception">Thrown when the delete operation fails.</exception>
        public Task<bool> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            try
            {
                var filePath = GetFilePath(id);
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete data entity from file storage.", ex);
            }
        }
    }
}