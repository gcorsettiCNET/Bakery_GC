using Bakery.Application.Commands.Products;
using Bakery.Application.DTOs;
using Bakery.Application.Queries.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bakery_GC.Controllers;

/// <summary>
/// Controller per la gestione prodotti usando pattern CQRS
/// Dimostra l'implementazione di Clean Architecture + CQRS con MediatR
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Ottiene tutti i prodotti con filtri e ordinamento
    /// </summary>
    /// <param name="categoryFilter">Filtro per categoria (opzionale)</param>
    /// <param name="isAvailable">Filtro per disponibilità (opzionale)</param>
    /// <param name="sortBy">Campo di ordinamento</param>
    /// <param name="sortDirection">Direzione ordinamento</param>
    /// <returns>Lista di prodotti</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] string? categoryFilter = null,
        [FromQuery] bool? isAvailable = null,
        [FromQuery] ProductSortBy sortBy = ProductSortBy.Name,
        [FromQuery] SortDirection sortDirection = SortDirection.Ascending)
    {
        try
        {
            var query = new GetAllProductsQuery
            {
                CategoryFilter = categoryFilter,
                IsAvailableFilter = isAvailable,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(new
            {
                success = true,
                data = result.Value,
                count = result.Value.Count(),
                message = "Products retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, new { error = "An error occurred while retrieving products" });
        }
    }

    /// <summary>
    /// Ottiene un prodotto specifico per ID
    /// </summary>
    /// <param name="id">ID del prodotto</param>
    /// <returns>Dettagli del prodotto</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        try
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return NotFound(new { error = result.Error });
            }

            return Ok(new
            {
                success = true,
                data = result.Value,
                message = "Product retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the product" });
        }
    }

    /// <summary>
    /// Crea un nuovo prodotto
    /// </summary>
    /// <param name="createProductDto">Dati del prodotto da creare</param>
    /// <returns>Prodotto creato</returns>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = CreateProductCommand.FromDto(createProductDto);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = result.Value.Id },
                new
                {
                    success = true,
                    data = result.Value,
                    message = "Product created successfully"
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, new { error = "An error occurred while creating the product" });
        }
    }

    /// <summary>
    /// Aggiorna un prodotto esistente
    /// </summary>
    /// <param name="id">ID del prodotto da aggiornare</param>
    /// <param name="updateCommand">Dati di aggiornamento</param>
    /// <returns>Prodotto aggiornato</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand updateCommand)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            updateCommand.ProductId = id;
            var result = await _mediator.Send(updateCommand);

            if (result.IsFailure)
            {
                return NotFound(new { error = result.Error });
            }

            return Ok(new
            {
                success = true,
                data = result.Value,
                message = "Product updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, new { error = "An error occurred while updating the product" });
        }
    }

    /// <summary>
    /// Elimina un prodotto
    /// </summary>
    /// <param name="id">ID del prodotto da eliminare</param>
    /// <returns>Conferma eliminazione</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var command = new DeleteProductCommand(id);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return NotFound(new { error = result.Error });
            }

            return Ok(new
            {
                success = true,
                message = "Product deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, new { error = "An error occurred while deleting the product" });
        }
    }

    /// <summary>
    /// Endpoint specifico per testare il pattern CQRS
    /// </summary>
    [HttpGet("cqrs-demo")]
    public async Task<IActionResult> CqrsDemo()
    {
        try
        {
            _logger.LogInformation("=== CQRS DEMO START ===");

            // Query: Recupera tutti i prodotti
            var getAllQuery = new GetAllProductsQuery
            {
                SortBy = ProductSortBy.Name,
                SortDirection = SortDirection.Ascending
            };

            var productsResult = await _mediator.Send(getAllQuery);

            // Command: Crea un nuovo prodotto (se non ci sono errori)
            ProductDto? newProduct = null;
            if (productsResult.IsSuccess)
            {
                var createCommand = new CreateProductCommand
                {
                    Name = $"CQRS Demo Product {DateTime.Now:HHmmss}",
                    Description = "Prodotto creato per dimostrare il pattern CQRS",
                    Price = 15.50m,
                    Category = "Pizza",
                    IsAvailable = true,
                    Ingredients = "Pomodoro, mozzarella, basilico",
                    Size = "Media",
                    IsSpicy = false
                };

                var createResult = await _mediator.Send(createCommand);
                if (createResult.IsSuccess)
                {
                    newProduct = createResult.Value;
                }
            }

            _logger.LogInformation("=== CQRS DEMO END ===");

            return Ok(new
            {
                success = true,
                demo = "CQRS Pattern with MediatR",
                patterns = new[]
                {
                    "Command Query Responsibility Segregation (CQRS)",
                    "MediatR Pipeline Behaviors",
                    "Performance Logging",
                    "Request/Response Logging",
                    "Clean Architecture"
                },
                operations = new
                {
                    query = new
                    {
                        operation = "GetAllProductsQuery",
                        productsCount = productsResult.IsSuccess ? productsResult.Value.Count() : 0,
                        success = productsResult.IsSuccess
                    },
                    command = new
                    {
                        operation = "CreateProductCommand",
                        productCreated = newProduct?.Name ?? "None",
                        success = newProduct != null
                    }
                },
                message = "CQRS demo completed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CQRS demo");
            return StatusCode(500, new { error = "An error occurred during CQRS demo" });
        }
    }
}