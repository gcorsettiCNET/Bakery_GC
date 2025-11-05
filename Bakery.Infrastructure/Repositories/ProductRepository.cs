using Bakery.Core.Common;
using Bakery.Core.Entities.Products;
using Bakery.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bakery.Infrastructure.Repositories;

/// <summary>
/// Implementazione del repository per Products con logica business specifica
/// </summary>
public class ProductRepository : Repository<Product, Guid>, IProductRepository
{
    public ProductRepository(DbContext context, ILogger<Repository<Product, Guid>> logger) 
        : base(context, logger)
    {
    }

    public async Task<Result<IEnumerable<Product>>> GetAvailableProductsByMarketAsync(Guid marketId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting available products for market {MarketId}", marketId);

            var products = await _dbSet
                .Where(p => p.MarketId == marketId && 
                           p.IsAvailable && 
                           !p.IsDeleted)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {Count} available products for market {MarketId}", products.Count, marketId);
            return Result<IEnumerable<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available products for market {MarketId}", marketId);
            return Result<IEnumerable<Product>>.Failure(ex);
        }
    }

    public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(ProductType productType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting products of type {ProductType}", productType);

            var products = await _dbSet
                .Where(p => p.ProductType == productType && !p.IsDeleted)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {Count} products of type {ProductType}", products.Count, productType);
            return Result<IEnumerable<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products of type {ProductType}", productType);
            return Result<IEnumerable<Product>>.Failure(ex);
        }
    }

    public async Task<Result<IEnumerable<Product>>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Result<IEnumerable<Product>>.Failure("Search term cannot be empty");
            }

            _logger.LogDebug("Searching products with term '{SearchTerm}'", searchTerm);

            var products = await _dbSet
                .Where(p => !p.IsDeleted &&
                           (p.Name.Contains(searchTerm) || 
                            (p.Description != null && p.Description.Contains(searchTerm))))
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {Count} products matching search term '{SearchTerm}'", products.Count, searchTerm);
            return Result<IEnumerable<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term '{SearchTerm}'", searchTerm);
            return Result<IEnumerable<Product>>.Failure(ex);
        }
    }

    public async Task<Result<IEnumerable<Product>>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        try
        {
            if (minPrice < 0 || maxPrice < minPrice)
            {
                return Result<IEnumerable<Product>>.Failure("Invalid price range");
            }

            _logger.LogDebug("Getting products in price range {MinPrice} - {MaxPrice}", minPrice, maxPrice);

            var products = await _dbSet
                .Where(p => p.Price >= minPrice && 
                           p.Price <= maxPrice && 
                           !p.IsDeleted)
                .OrderBy(p => p.Price)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {Count} products in price range {MinPrice} - {MaxPrice}", products.Count, minPrice, maxPrice);
            return Result<IEnumerable<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products in price range {MinPrice} - {MaxPrice}", minPrice, maxPrice);
            return Result<IEnumerable<Product>>.Failure(ex);
        }
    }

    public async Task<Result<IEnumerable<Product>>> GetMostOrderedProductsAsync(int take = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting top {Take} most ordered products", take);

            // Per ora torniamo prodotti ordinati per nome
            // Quando avremo OrderItems potremo implementare la logica corretta
            var products = await _dbSet
                .Where(p => p.IsAvailable && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .Take(take)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Retrieved {Count} most ordered products", products.Count);
            return Result<IEnumerable<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting most ordered products");
            return Result<IEnumerable<Product>>.Failure(ex);
        }
    }

    public async Task<Result<bool>> CanBeOrderedAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if product {ProductId} can be ordered", productId);

            var product = await _dbSet.FindAsync(new object[] { productId }, cancellationToken);
            
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found", productId);
                return Result<bool>.Success(false);
            }

            var canBeOrdered = product.CanBeOrdered();
            _logger.LogDebug("Product {ProductId} can be ordered: {CanBeOrdered}", productId, canBeOrdered);
            
            return Result<bool>.Success(canBeOrdered);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if product {ProductId} can be ordered", productId);
            return Result<bool>.Failure(ex);
        }
    }
}