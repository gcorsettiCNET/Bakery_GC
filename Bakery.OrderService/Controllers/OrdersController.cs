using Microsoft.AspNetCore.Mvc;

namespace Bakery.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    /// <summary>
    /// Ottiene tutti gli ordini
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new[] 
        { 
            new { Id = 1, Product = "Croissant", Quantity = 5 },
            new { Id = 2, Product = "Baguette", Quantity = 3 }
        });
    }

    /// <summary>
    /// Ottiene un ordine per ID
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { Id = id, Product = "Croissant", Quantity = 5 });
    }

    /// <summary>
    /// Crea un nuovo ordine
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest request)
    {
        return CreatedAtAction(nameof(GetById), new { id = 1 }, 
            new { Id = 1, request.Product, request.Quantity });
    }
}

public record CreateOrderRequest(string Product, int Quantity);
