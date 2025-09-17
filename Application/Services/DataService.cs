using AutoMapper;
using DataService.Application.DTOs;
using DataService.Infrastructure.StorageProviders;
using DataService.Models.Entities;
using DataService.Models.Exceptions;
using Models.Exceptions;
using System;

namespace DataService.Application.Services
{
    /// <summary>
    /// Provides CRUD operations for <see cref="DataEntity"/> 
    /// using multiple storage providers via a factory.
    /// </summary>
    public class DataService : IDataService
    {
        private readonly IStorageProviderFactory _factory;
        private readonly IMapper _mapper;

        public DataService(IStorageProviderFactory factory, IMapper mapper)
        {
            _factory = factory;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an entity by its ID. 
        /// Checks providers in order until found, 
        /// and refreshes earlier providers (cache/file).
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>The entity DTO, or null if not found.</returns>
        public async Task<DataDto?> GetByIdAsync(string id)
        {
            try
            {
                foreach (var provider in _factory.GetAll())
                {
                    var entity = await provider.GetAsync(id);
                    if (entity != null)
                    {
                        foreach (var earlier in _factory.GetProvidersBefore(provider))
                            await earlier.SaveAsync(entity);

                        return _mapper.Map<DataDto>(entity);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                // TODO: Add logging here
                return null;
            }
        }

        /// <summary>
        /// Saves a new entity across all providers.
        /// </summary>
        /// <param name="value">The entity value to save.</param>
        /// <returns>The ID of the newly saved entity.</returns>
        public async Task<string> SaveAsync(string value)
        {
            try
            {
                var entity = new DataEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Value = value,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var provider in _factory.GetAll())
                {
                    if (!await provider.SaveAsync(entity))
                        throw new SaveOperationFailedException($"Failed to save to {provider.GetType().Name}");
                }

                return entity.Id;
            }
            catch (Exception ex)
            {
                // TODO: Add logging here
                return string.Empty;
            }
        }

        /// <summary>
        /// Updates an existing entity in all providers.
        /// </summary>
        /// <param name="id">The ID of the entity to update.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The ID of the updated entity.</returns>
        public async Task<string> UpdateAsync(string id, string newValue)
        {
            try
            {
                var dto = await GetByIdAsync(id);
                if (dto == null)
                    throw new NotFoundException($"Entity with id {id} not found.");

                var entity = _mapper.Map<DataEntity>(dto);
                entity.Value = newValue;
                entity.CreatedAt = DateTime.UtcNow;

                foreach (var provider in _factory.GetAll())
                {
                    if (!await provider.UpdateAsync(entity))
                        throw new SaveOperationFailedException($"Failed to update in {provider.GetType().Name}");
                }

                return entity.Id;
            }
            catch (Exception ex)
            {
                // TODO: Add logging here
                return string.Empty;
            }
        }

        /// <summary>
        /// Deletes an entity from all providers.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>True if delete succeeded in all providers, otherwise false.</returns>
        public async Task<bool> DeleteAsync(string id)
        {
            bool success = true;

            try
            {
                foreach (var provider in _factory.GetAll())
                {
                    try
                    {
                        await provider.DeleteAsync(id);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Add logging here
                        success = false;
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                // TODO: Add logging here
                return false;
            }
        }
    }
}
