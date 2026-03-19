using CustomerApi.DTOs;
using CustomerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] string? name,
        [FromQuery] string? city,
        [FromQuery] string? country,
        [FromQuery] bool? isActive,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _customerService.GetCustomersAsync(
            name, city, country, isActive, sortBy, sortDir, pageNumber, pageSize);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);

        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _customerService.CreateCustomerAsync(dto);

        if (!result.Success && result.Error == "EMAIL_EXISTS")
            return Conflict(new { message = "Email already exists." });

        return CreatedAtAction(nameof(GetById), new { id = result.Customer!.Id }, result.Customer);
    }

    [HttpPut("{id:int}")]

    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _customerService.UpdateCustomerAsync(id, dto);

        if (!result.Success && result.Error == "NOT_FOUND")
            return NotFound();

        if (!result.Success && result.Error == "EMAIL_EXISTS")
            return Conflict(new { message = "Email already exists." });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _customerService.SoftDeleteCustomerAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost("bulk-deactivate")]
    public async Task<IActionResult> BulkDeactivate([FromBody] BulkDeactivateDto dto)
    {
        if (dto.Ids == null || dto.Ids.Count == 0)
            return BadRequest(new { message = "Ids array cannot be empty." });

        if (dto.Ids.Count > 1000)
            return BadRequest(new { message = "Maximum 1000 ids is allowed." });

        var updatedCount = await _customerService.BulkDeactivateAsync(dto.Ids);

        return Ok(new { updatedCount });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _customerService.GetStatsAsync();
        return Ok(stats);
    }
}