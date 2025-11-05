using Bakery.Application.DTOs;
using Bakery.Core.Common;
using MediatR;

namespace Bakery.Application.Queries.Products;

/// <summary>
/// Query per ottenere prodotti per market specifico
/// </summary>
public class GetProductsByMarketQuery : IRequest<Result<IEnumerable<ProductSummaryDto>>>
{
    public Guid MarketId { get; set; }
    
    /// <summary>
    /// Filtro opzionale per categoria prodotto
    /// </summary>
    public string? CategoryFilter { get; set; }
    
    public GetProductsByMarketQuery(Guid marketId)
    {
        MarketId = marketId;
    }
}