using AutoMapper;
using DataService.Application.DTOs;
using DataService.Infrastructure.StorageProviders;
using DataService.Models.Entities;
using DataService.Models.Enums;
using DataService.Models.Exceptions;
using Models.Exceptions;
using System.Threading.Tasks.Dataflow;

namespace DataService.Application.Services
{
    /// <summary>
    /// Provides data management services with multi-tier storage support including cache, file, and database providers.
    /// Implements a layered caching strategy where data is searched from cache first, then file storage, and finally database.
    /// </summary>
    public class DataService : IDataService
    {
        private readonly IStorageProviderFactory _factory;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the DataService class.
        /// </summary>
        /// <param name="factory">The storage provider factory used to create storage providers for different storage types.</param>
        /// <param name="mapper">The AutoMapper instance used for mapping between entities and DTOs.</param>
        public DataService(IStorageProviderFactory factory, IMapper mapper)
        {
            _factory = factory;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a data entity by its unique identifier from the multi-tier storage system.
        /// Searches cache first, then file storage, then database, and refreshes upper-level storage as needed.
        /// </summary>
        /// <param name="id">The unique identifier of the data entity to retrieve.</param>
        /// <returns>A <see cref="DataDto"/> representing the found data entity, or null if not found or an error occurs.</returns>
        public async Task<DataDto> GetByIdAsync(string id)
        {
            try
            {
                var searchResult = await SearchDataEntityById(id);

                if (searchResult != null)
                {
                    if (!await RefreshDataInStorage(searchResult))
                    {
                        // Todo: Log warning that refresh failed
                    }

                    return _mapper.Map<DataDto>(searchResult.Entity);
                }

                return null;
            }
            catch (Exception ex)
            {
                // Todo: Log exception
                return null;
            }
        }

        /// <summary>
        /// Saves new data to all storage providers (cache, file, and database) in a transactional manner.
        /// If any storage operation fails, previously saved data is rolled back from successful providers.
        /// </summary>
        /// <param name="data">The string data to be saved.</param>
        /// <returns>The unique identifier of the saved data entity, or an empty string if the operation fails.</returns>
        /// <exception cref="SaveOperationFailedException">Thrown when saving to any storage provider fails.</exception>
        public async Task<string> SaveAsync(string data)
        {
            try
            {
                var dataItem = CreateDataEntity(data);
                if (await SaveToAllProvidersAsync(dataItem))
                {
                    return dataItem.Id;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Todo: Log exception
                return string.Empty;
            }
        }

        /// <summary>
        /// Updates an existing data entity across all storage providers.
        /// The entity must exist in the system before it can be updated.
        /// </summary>
        /// <param name="id">The unique identifier of the data entity to update.</param>
        /// <param name="data">The new data value to store.</param>
        /// <returns>The unique identifier of the updated data entity, or an empty string if the operation fails.</returns>
        /// <exception cref="NotFoundException">Thrown when the specified entity ID is not found in the system.</exception>
        public async Task<string> UpdateAsync(string id, string data)
        {
            try
            {
                var searchResult = await SearchDataEntityById(id);
                if (searchResult == null)
                {
                    throw new NotFoundException("Entry with id: " + id + " wasn't found in the system. Update operation failed");
                }

                searchResult.Entity.Value = data;
                searchResult.Entity.CreatedAt = DateTime.Now;
                searchResult.FoundIn = null;    // Has to be updated in all providers

                if (!await UpdateDataInProvidersAsync(searchResult.Entity, new List<IStorageProvider>
                {
                    _factory.GetStorageProvider(StorageType.Cache),
                    _factory.GetStorageProvider(StorageType.File),
                    _factory.GetStorageProvider(StorageType.Database)
                }))
                {
                    // Todo: Log warning that update failed
                    return string.Empty;
                }

                return searchResult.Entity.Id;
            }
            catch (Exception ex)
            {
                // Todo: Log exception
                return string.Empty;
            }

        }

        /// <summary>
        /// Searches for a data entity by its ID across all storage providers in priority order (cache -> file -> database).
        /// Returns the first match found along with information about which storage provider contained the data.
        /// </summary>
        /// <param name="id">The unique identifier to search for.</param>
        /// <returns>A <see cref="SearchResult"/> containing the found entity and storage location, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the ID parameter is null or empty.</exception>
        private async Task<SearchResult> SearchDataEntityById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("ID cannot be null or empty", nameof(id));
            }

            var searchResult = new SearchResult();
            var cache = _factory.GetStorageProvider(StorageType.Cache);
            var cached = await cache.GetAsync(id);
            if (cached != null)
            {
                searchResult.Entity = cached;
                searchResult.FoundIn = StorageType.Cache;
                return searchResult;
            }

            var file = _factory.GetStorageProvider(StorageType.File);
            var fileItem = await file.GetAsync(id);
            if (fileItem != null)
            {
                //await cache.SaveAsync(fileItem, TimeSpan.FromMinutes(10));
                searchResult.Entity = fileItem;
                searchResult.FoundIn = StorageType.File;
                return searchResult;
            }

            var db = _factory.GetStorageProvider(StorageType.Database);
            var dbItem = await db.GetAsync(id);
            if (dbItem != null)
            {
                //await file.SaveAsync(dbItem, TimeSpan.FromMinutes(30));
                //await cache.SaveAsync(dbItem, TimeSpan.FromMinutes(10));
                searchResult.Entity = dbItem;
                searchResult.FoundIn = StorageType.Database;
                return searchResult;
            }

            return null;
        }

        /// <summary>
        /// Saves a data entity to all storage providers (cache, file, and database) with rollback capability.
        /// If any save operation fails, successfully saved data is cleaned up from other providers.
        /// </summary>
        /// <param name="entity">The data entity to save across all providers.</param>
        /// <returns>True if the entity was successfully saved to all providers; otherwise, false.</returns>
        /// <exception cref="SaveOperationFailedException">Thrown when saving to any storage provider fails.</exception>
        private async Task<bool> SaveToAllProvidersAsync(DataEntity entity)
        {
            var fileProvider = _factory.GetStorageProvider(StorageType.File);
            var cacheProvider = _factory.GetStorageProvider(StorageType.Cache);

            var cacheResult = await SaveToCacheProviderAsync(entity);
            if (!cacheResult)
            {
                throw new SaveOperationFailedException("Save data to cache failed");
            }

            var fileResult = await SaveToFileProviderAsync(entity);
            if (!fileResult)
            {
                try
                {
                    await DeleteDataFromProvidersAsync(entity.Id, new List<IStorageProvider> { cacheProvider });
                }
                catch (ArgumentNullException ex)
                {
                    // Todo: Log exception
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Todo: Log exception
                }
                catch (Exception ex)
                {
                    // Todo: Log exception
                }

                throw new SaveOperationFailedException("Save data to file failed");
            }

            var dbResult = await SaveToDbProviderAsync(entity);
            if (!dbResult)
            {
                try
                {
                    await DeleteDataFromProvidersAsync(entity.Id, new List<IStorageProvider> { cacheProvider, fileProvider });
                }
                catch (ArgumentNullException ex)
                {
                    // Todo: Log exception
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Todo: Log exception
                }
                catch (Exception ex)
                {
                    // Todo: Log exception
                }

                throw new SaveOperationFailedException("Save data to DB failed");
            }
            return true;
        }

        /// <summary>
        /// Saves a data entity to the cache storage provider.
        /// </summary>
        /// <param name="entity">The data entity to save to cache.</param>
        /// <returns>True if the save operation was successful; otherwise, false.</returns>
        private async Task<bool> SaveToCacheProviderAsync(DataEntity entity)
        {
            var cacheProvider = _factory.GetStorageProvider(StorageType.Cache);
            return await cacheProvider.SaveAsync(entity);
        }

        /// <summary>
        /// Saves a data entity to the file storage provider.
        /// </summary>
        /// <param name="entity">The data entity to save to file storage.</param>
        /// <returns>True if the save operation was successful; otherwise, false.</returns>
        private async Task<bool> SaveToFileProviderAsync(DataEntity entity)
        {
            var fileProvider = _factory.GetStorageProvider(StorageType.File);
            return await fileProvider.SaveAsync(entity);
        }

        /// <summary>
        /// Saves a data entity to the database storage provider.
        /// </summary>
        /// <param name="entity">The data entity to save to the database.</param>
        /// <returns>True if the save operation was successful; otherwise, false.</returns>
        private async Task<bool> SaveToDbProviderAsync(DataEntity entity)
        {
            var dbProvider = _factory.GetStorageProvider(StorageType.Database);
            return await dbProvider.SaveAsync(entity);
        }

        /// <summary>
        /// Deletes data from multiple storage providers by entity ID.
        /// Used for rollback operations when save or update operations fail.
        /// </summary>
        /// <param name="id">The unique identifier of the data to delete.</param>
        /// <param name="providers">The list of storage providers from which to delete the data.</param>
        /// <exception cref="ArgumentNullException">Thrown when the providers list is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the providers list is empty.</exception>
        private async Task DeleteDataFromProvidersAsync(string id, List<IStorageProvider> providers)
        {
            if (providers == null)
            {
                throw new ArgumentNullException("Providers list cannot be null", nameof(providers));
            }
            if (providers.Count == 0)
            {
                throw new ArgumentOutOfRangeException("Providers list cannot be empty", nameof(providers));
            }

            foreach (var provider in providers)
            {
                try
                {
                    await provider.DeleteAsync(id);
                }
                catch (Exception ex)
                {
                    // Todo: Log exception for specific provider
                }
            }
        }

        /// <summary>
        /// Updates a data entity across multiple storage providers.
        /// If any provider fails to update, the entire operation is considered failed.
        /// </summary>
        /// <param name="entity">The data entity to update.</param>
        /// <param name="providers">The list of storage providers to update the entity in.</param>
        /// <returns>True if the entity was successfully updated in all providers; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the entity or providers list is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the providers list is empty.</exception>
        private async Task<bool> UpdateDataInProvidersAsync(DataEntity entity, List<IStorageProvider> providers)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Data entity cannot be null", nameof(entity));
            }
            if (providers == null)
            {
                throw new ArgumentNullException("Providers list cannot be null", nameof(providers));
            }
            if (providers.Count == 0)
            {
                throw new ArgumentOutOfRangeException("Providers list cannot be empty", nameof(providers));
            }

            foreach (var provider in providers)
            {
                if (!await provider.UpdateAsync(entity))
                {
                    // Todo: Log warning that update failed for specific provider
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Refreshes data in higher-priority storage providers based on where the data was originally found.
        /// For example, if data was found in the database, it will be saved to both file and cache storage.
        /// If data was found in file storage, it will be saved to cache storage.
        /// </summary>
        /// <param name="searchResult">The search result containing the entity and its original storage location.</param>
        /// <returns>True if the refresh operation was successful; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the search result is null.</exception>
        private async Task<bool> RefreshDataInStorage(SearchResult searchResult)
        {
            if (searchResult == null)
            {
                throw new ArgumentNullException("Search result cannot be null", nameof(searchResult));
            }

            if (searchResult.FoundIn == StorageType.Database)
            {
                try
                {
                    var fileRes = await SaveToFileProviderAsync(searchResult.Entity);
                    var cacheRes = await SaveToCacheProviderAsync(searchResult.Entity);
                    if (fileRes && cacheRes)
                    {
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    //Todo: Log exception
                    return false;
                }
            }

            if (searchResult.FoundIn == StorageType.File)
            {
                try
                {
                    return await SaveToCacheProviderAsync(searchResult.Entity);
                }
                catch (Exception ex)
                {
                    //Todo: Log exception
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a new data entity with a unique identifier and timestamp.
        /// </summary>
        /// <param name="data">The string data to be stored in the entity.</param>
        /// <returns>A new <see cref="DataEntity"/> with a generated GUID, current timestamp, and the provided data.</returns>
        private DataEntity CreateDataEntity(string data)
        {
            return new DataEntity
            {
                Id = Guid.NewGuid().ToString(),
                Value = data,
                CreatedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Represents the result of a search operation, containing the found entity and its storage location.
        /// Used internally to track where data was found during multi-tier storage searches.
        /// </summary>
        private class SearchResult
        {
            /// <summary>
            /// Gets or sets the data entity that was found during the search operation.
            /// </summary>
            public DataEntity Entity { get; set; }

            /// <summary>
            /// Gets or sets the storage type where the entity was found.
            /// Null if the entity needs to be updated in all providers.
            /// </summary>
            public StorageType? FoundIn { get; set; }
        }
    }
}