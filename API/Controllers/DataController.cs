using DataService.Application.DTOs;
using DataService.Application.Services;
using DataService.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataService.Api.Controllers;

[ApiController]
[Route("data")]
public class DataController : ControllerBase
{
    private readonly IDataService _dataService;

    public DataController(IDataService dataService)
    {
        _dataService = dataService;
    }

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
