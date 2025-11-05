using Bakery.Core.Common;
using Bakery.Core.Entities.Products;

namespace Bakery.Core.Interfaces.Repositories;

/// <summary>
/// Repository specifico per Products con metodi business-oriented
/// </summary>
public interface IProductRepository : IRepository<Product, Guid>
{
    /// <summary>
    /// Ottiene prodotti disponibili per un market specifico
    /// </summary>
    Task<Result<IEnumerable<Product>>> GetAvailableProductsByMarketAsync(Guid marketId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ottiene prodotti per tipo
    /// </summary>
    Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(ProductType productType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cerca prodotti per nome o descrizione
    /// </summary>
    Task<Result<IEnumerable<Product>>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ottiene prodotti in una fascia di prezzo
    /// </summary>
    Task<Result<IEnumerable<Product>>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ottiene i prodotti più ordinati
    /// </summary>
    Task<Result<IEnumerable<Product>>> GetMostOrderedProductsAsync(int take = 10, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Verifica se un prodotto può essere ordinato
    /// </summary>
    Task<Result<bool>> CanBeOrderedAsync(Guid productId, CancellationToken cancellationToken = default);
}