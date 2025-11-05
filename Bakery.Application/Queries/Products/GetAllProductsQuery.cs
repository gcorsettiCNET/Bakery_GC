using Bakery.Application.DTOs;
using Bakery.Core.Common;
using MediatR;

namespace Bakery.Application.Queries.Products;

/// <summary>
/// Query per ottenere tutti i prodotti
/// Implementa il pattern CQRS per operazioni di lettura
/// </summary>
public class GetAllProductsQuery : IRequest<Result<IEnumerable<ProductSummaryDto>>>
{
    /// <summary>
    /// Filtro opzionale per categoria prodotto
    /// </summary>
    public string? CategoryFilter { get; set; }
    
    /// <summary>
    /// Filtro per prodotti disponibili/non disponibili
    /// </summary>
    public bool? IsAvailableFilter { get; set; }
    
    /// <summary>
    /// Ordinamento dei risultati
    /// </summary>
    public ProductSortBy SortBy { get; set; } = ProductSortBy.Name;
    
    /// <summary>
    /// Direzione dell'ordinamento
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
}

/// <summary>
/// Opzioni di ordinamento per i prodotti
/// </summary>
public enum ProductSortBy
{
    Name,
    Price,
    Category,
    CreatedAt
}

/// <summary>
/// Direzione dell'ordinamento
/// </summary>
public enum SortDirection
{
    Ascending,
    Descending
}