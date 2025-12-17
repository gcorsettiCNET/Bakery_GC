using Bakery.Application.Queries.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bakery_GC.Controllers;

/// <summary>
/// Controller per testare Clean Architecture + CQRS
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TestController> _logger;

    public TestController(IMediator mediator, ILogger<TestController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new 
        {
            success = true,
            message = "Clean Architecture + CQRS API is running!",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get all products using CQRS
    /// </summary>
    [HttpGet("products")]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var query = new GetAllProductsQuery();
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(new 
                {
                    success = true,
                    count = result.Value.Count(),
                    products = result.Value
                });
            }

            return BadRequest(new { success = false, error = result.Error });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get product by ID using CQRS
    /// </summary>
    [HttpGet("products/{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        try
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(new { success = true, product = result.Value });
            }

            return NotFound(new { success = false, error = result.Error });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}
