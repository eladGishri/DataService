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
    public class DataService : IDataService
    {
        private readonly IStorageProviderFactory _factory;
        private readonly IMapper _mapper;

        public DataService(IStorageProviderFactory factory, IMapper mapper)
        {
            _factory = factory;
            _mapper = mapper;
        }

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

        private async Task<bool> SaveToCacheProviderAsync(DataEntity entity)
        {
            var cacheProvider = _factory.GetStorageProvider(StorageType.Cache);
            return await cacheProvider.SaveAsync(entity);
        }

        private async Task<bool> SaveToFileProviderAsync(DataEntity entity)
        {
            var fileProvider = _factory.GetStorageProvider(StorageType.File);
            return await fileProvider.SaveAsync(entity);
        }

        private async Task<bool> SaveToDbProviderAsync(DataEntity entity)
        {
            var dbProvider = _factory.GetStorageProvider(StorageType.Database);
            return await dbProvider.SaveAsync(entity);
        }

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

        private DataEntity CreateDataEntity(string data)
        {
            return new DataEntity
            {
                Id = Guid.NewGuid().ToString(),
                Value = data,
                CreatedAt = DateTime.Now
            };
        }

        private class SearchResult
        {
            public DataEntity Entity { get; set; }
            public StorageType? FoundIn { get; set; }
        }
    }
}
