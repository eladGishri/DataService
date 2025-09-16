using DataService.Application.DTOs;
using DataService.Models.Entities;

namespace DataService.Application.Services
{
    /// <summary>
    /// Defines the contract for a service that handles business logic and operations
    /// related to <see cref="DataDto"/> and <see cref="DataEntity"/>.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Asynchronously retrieves a <see cref="DataDto"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the data item.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved <see cref="DataDto"/>.</returns>
        Task<DataDto> GetByIdAsync(string id);

        /// <summary>
        /// Asynchronously saves a new data item represented as a raw string.
        /// </summary>
        /// <param name="data">The raw data to be saved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the saved item.</returns>
        Task<string> SaveAsync(string data);

        /// <summary>
        /// Asynchronously updates an existing data item identified by its ID using the provided raw data.
        /// </summary>
        /// <param name="id">The unique identifier of the data item to update.</param>
        /// <param name="data">The updated raw data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the updated item.</returns>
        Task<string> UpdateAsync(string id, string data);
    }
}
