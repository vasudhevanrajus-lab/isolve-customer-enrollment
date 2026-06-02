using CustomerEnrollment.Api.Models;
using CustomerEnrollment.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerEnrollment.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService service, ILogger<CustomersController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Enroll([FromBody] EncryptedCustomerDto dto, CancellationToken ct)
    {
        if (dto == null)
            return BadRequest(new { error = "Payload is required." });

        try
        {
            var id = await _service.EnrollAsync(dto, ct);
            return CreatedAtAction(nameof(Get),
                new { customerId = id },
                new EnrollResponse { CustomerId = id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Enrollment failed");
            return Problem("Unable to process enrollment. Please try again.");
        }
    }

    [HttpGet("{customerId:int}")]
    public async Task<IActionResult> Get(int customerId, CancellationToken ct)
    {
        var customer = await _service.GetAsync(customerId, ct);
        return customer == null ? NotFound() : Ok(customer);
    }
}
