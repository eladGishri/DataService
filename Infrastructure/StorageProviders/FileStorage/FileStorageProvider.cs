using System.Text.Json;
using DataService.Models.Entities;
using DataService.Infrastructure.StorageProviders;
using DataService.Models.Enums;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.StorageProviders.FileStorage
{
    public class FileStorageProvider : IStorageProvider, IFileProvider
    {
        public StorageType Type => StorageType.File;
        private const int _fileExpirationMinutes = 30;
        private readonly string _storageDirectory;

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

        public Task<bool> UpdateAsync(DataEntity item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
                throw new ArgumentNullException(nameof(item));

            return SaveAsync(item);
        }

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