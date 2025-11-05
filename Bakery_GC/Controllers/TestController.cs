using Bakery.Core.Interfaces;
using Bakery.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Bakery_GC.Controllers;

/// <summary>
/// Controller per testare la nuova architettura
/// Dimostra l'uso di Repository Pattern + Unit of Work
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TestController> _logger;

    public TestController(
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<TestController> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Test base: ottiene tutti i prodotti
    /// </summary>
    [HttpGet("products")]
    public async Task<IActionResult> GetAllProducts()
    {
        _logger.LogInformation("Testing GetAllProducts...");

        var result = await _productRepository.GetAllAsync();
        
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get products: {Error}", result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation("Successfully retrieved {Count} products", result.Value.Count());
        return Ok(new 
        { 
            success = true,
            count = result.Value.Count(),
            products = result.Value.Select(p => new 
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.ProductType,
                p.IsAvailable
            })
        });
    }

    /// <summary>
    /// Test Repository specifico: prodotti per Market
    /// </summary>
    [HttpGet("products/by-market/{marketId:guid}")]
    public async Task<IActionResult> GetProductsByMarket(Guid marketId)
    {
        _logger.LogInformation("Testing GetProductsByMarket for {MarketId}...", marketId);

        var result = await _productRepository.GetAvailableProductsByMarketAsync(marketId);
        
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get products by market: {Error}", result.Error);
            return BadRequest(new { error = result.Error });
        }

        return Ok(new 
        { 
            success = true,
            marketId,
            count = result.Value.Count(),
            products = result.Value.Select(p => new 
            {
                p.Id,
                p.Name,
                p.Price,
                p.ProductType
            })
        });
    }

    /// <summary>
    /// Test Unit of Work: operazione transazionale
    /// </summary>
    [HttpPost("test-transaction")]
    public async Task<IActionResult> TestTransaction()
    {
        _logger.LogInformation("Testing Unit of Work transaction...");

        try
        {
            // Inizia transazione
            var beginResult = await _unitOfWork.BeginTransactionAsync();
            if (beginResult.IsFailure)
            {
                return BadRequest(new { error = "Failed to begin transaction", details = beginResult.Error });
            }

            // Test: conta prodotti prima
            var countBeforeResult = await _productRepository.CountAsync();
            if (countBeforeResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BadRequest(new { error = "Failed to count products", details = countBeforeResult.Error });
            }

            var countBefore = countBeforeResult.Value;

            // Simula operazione che potrebbe fallire
            var random = new Random();
            if (random.Next(1, 3) == 1) // 50% chance di fallimento simulato
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Ok(new 
                { 
                    success = false,
                    message = "Transaction rolled back (simulated failure)",
                    productCountBefore = countBefore
                });
            }

            // Commit transazione
            var commitResult = await _unitOfWork.CommitTransactionAsync();
            if (commitResult.IsFailure)
            {
                return BadRequest(new { error = "Failed to commit transaction", details = commitResult.Error });
            }

            return Ok(new 
            { 
                success = true,
                message = "Transaction completed successfully",
                productCountBefore = countBefore,
                hasPendingChanges = _unitOfWork.HasPendingChanges(),
                pendingChangesCount = _unitOfWork.GetPendingChangesCount()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during transaction test");
            await _unitOfWork.RollbackTransactionAsync();
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Test Customer Repository: VIP customers
    /// </summary>
    [HttpGet("customers/vip")]
    public async Task<IActionResult> GetVipCustomers()
    {
        _logger.LogInformation("Testing GetVipCustomers...");

        var result = await _customerRepository.GetVipCustomersAsync();
        
        if (result.IsFailure)
        {
            _logger.LogError("Failed to get VIP customers: {Error}", result.Error);
            return BadRequest(new { error = result.Error });
        }

        return Ok(new 
        { 
            success = true,
            count = result.Value.Count(),
            vipCustomers = result.Value.Select(c => new 
            {
                c.Id,
                c.FullName,
                c.Email,
                c.TotalSpent,
                c.IsVip,
                VipDiscount = c.GetVipDiscountPercentage()
            })
        });
    }

    /// <summary>
    /// Test Result Pattern: operazione che può fallire
    /// </summary>
    [HttpGet("test-result-pattern/{productId:guid}")]
    public async Task<IActionResult> TestResultPattern(Guid productId)
    {
        _logger.LogInformation("Testing Result Pattern with product {ProductId}...", productId);

        var result = await _productRepository.GetByIdAsync(productId);
        
        if (result.IsFailure)
        {
            _logger.LogWarning("Product not found or error: {Error}", result.Error);
            return NotFound(new 
            { 
                success = false,
                error = result.Error,
                message = "This demonstrates Result Pattern - no exceptions thrown!"
            });
        }

        var product = result.Value;
        return Ok(new 
        { 
            success = true,
            message = "Result Pattern working correctly!",
            product = new 
            {
                product.Id,
                product.Name,
                product.Price,
                product.ProductType,
                CanBeOrdered = product.CanBeOrdered(),
                DiscountedPrice = product.CalculateDiscountedPrice(10) // 10% sconto
            }
        });
    }

    /// <summary>
    /// Health check dell'architettura
    /// </summary>
    [HttpGet("health")]
    public async Task<IActionResult> HealthCheck()
    {
        try
        {
            // Test connessione database
            var productsResult = await _productRepository.CountAsync();
            var customersResult = await _customerRepository.CountAsync();

            if (productsResult.IsFailure || customersResult.IsFailure)
            {
                return StatusCode(503, new 
                { 
                    status = "Unhealthy",
                    database = "Connection failed"
                });
            }

            return Ok(new 
            { 
                status = "Healthy",
                architecture = "Clean Architecture with Repository Pattern + Unit of Work",
                database = "Connected",
                productsCount = productsResult.Value,
                customersCount = customersResult.Value,
                designPatterns = new[] 
                {
                    "Repository Pattern",
                    "Unit of Work",
                    "Result Pattern",
                    "Dependency Injection",
                    "Clean Architecture"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new 
            { 
                status = "Unhealthy",
                error = ex.Message
            });
        }
    }
}