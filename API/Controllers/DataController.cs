using DataService.Application.DTOs;
using DataService.Application.Services;
using DataService.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataService.Api.Controllers
{
    /// <summary>
    /// RESTful API controller for managing data entities in the multi-tier storage system.
    /// Provides endpoints for creating, reading, and updating data with role-based authorization.
    /// </summary>
    [ApiController]
    [Route("data")]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataController"/> class.
        /// </summary>
        /// <param name="dataService">The data service instance used for business logic operations.</param>
        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Retrieves a data entity by its unique identifier.
        /// Searches across all storage providers (cache, file, database) and returns the found entity as a DTO.
        /// </summary>
        /// <param name="id">The unique identifier of the data entity to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing:
        /// - 200 OK with the <see cref="DataDto"/> if found
        /// - 400 Bad Request if the ID is null or empty
        /// - 404 Not Found if the entity doesn't exist
        /// </returns>
        /// <response code="200">Returns the requested data entity</response>
        /// <response code="400">The provided ID is invalid</response>
        /// <response code="401">Authentication is required</response>
        /// <response code="403">User lacks required permissions (User or Admin role required)</response>
        /// <response code="404">The data entity was not found</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID cannot be null or empty.");
            }

            var item = await _dataService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        /// <summary>
        /// Creates a new data entity in all storage providers.
        /// Generates a unique identifier and saves the data across cache, file, and database storage.
        /// </summary>
        /// <param name="data">The string data content to be stored in the new entity.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing:
        /// - 201 Created with the generated entity ID in the response body
        /// - 400 Bad Request if the data is null or empty
        /// - 500 Internal Server Error if the save operation fails
        /// </returns>
        /// <response code="201">Data entity created successfully, returns the generated ID</response>
        /// <response code="400">The provided data is invalid</response>
        /// <response code="401">Authentication is required</response>
        /// <response code="403">User lacks required permissions (Admin role required)</response>
        /// <response code="500">An error occurred during the save operation</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InsertData([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest("Data cannot be null or empty.");
            }

            var id = await _dataService.SaveAsync(data);
            return Created(string.Empty, id);
        }

        /// <summary>
        /// Updates an existing data entity across all storage providers.
        /// The entity must exist in the system before it can be updated.
        /// Updates the entity's value and timestamp in all storage locations.
        /// </summary>
        /// <param name="id">The unique identifier of the data entity to update.</param>
        /// <param name="data">The new string data content to replace the existing value.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing:
        /// - 204 No Content if the update was successful
        /// - 400 Bad Request if the ID or data is null or empty
        /// - 500 Internal Server Error if the update operation fails
        /// </returns>
        /// <response code="204">Data entity updated successfully</response>
        /// <response code="400">The provided ID or data is invalid</response>
        /// <response code="401">Authentication is required</response>
        /// <response code="403">User lacks required permissions (Admin role required)</response>
        /// <response code="404">The data entity was not found (handled by service layer)</response>
        /// <response code="500">An error occurred during the update operation</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateData(string id, [FromBody] string data)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(data))
            {
                return BadRequest("Data cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(await _dataService.UpdateAsync(id, data)))
            {
                return StatusCode(500, "An unexpected error occurred.");
            }

            return NoContent();
        }
    }
}