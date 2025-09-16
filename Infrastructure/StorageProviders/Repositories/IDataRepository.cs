using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService.Models.Entities;

namespace Infrastructure.StorageProviders.Repositories
{
    /// <summary>
    /// Defines the contract for a data repository that handles persistence operations for <see cref="DataEntity"/>.
    /// </summary>
public interface IDataRepository
{
        // Future functionality expansion

        /// <summary>
        /// Asynchronously retrieves a <see cref="DataEntity"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved <see cref="DataEntity"/>.</returns>
        // Task<DataEntity> GetByIdAsync(string id);

        /// <summary>
        /// Asynchronously inserts a new <see cref="DataEntity"/> into the repository.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the insert was successful.</returns>
        // Task<bool> InsertAsync(DataEntity entity);

        /// <summary>
        /// Asynchronously updates an existing <see cref="DataEntity"/> in the repository.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the update was successful.</returns>
        // Task<bool> UpdateAsync(DataEntity entity);
    }
}