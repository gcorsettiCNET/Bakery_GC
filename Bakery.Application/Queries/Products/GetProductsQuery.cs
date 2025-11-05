using Bakery.Application.Common.Models;
using Bakery.Application.DTOs;
using Bakery.Core.Common;
using MediatR;

namespace Bakery.Application.Queries.Products;

/// <summary>
/// Query to get paginated and filtered products
/// </summary>
public class GetProductsQuery : IRequest<Result<PagedList<ProductDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? CategoryFilter { get; set; }
    public bool? IsAvailableFilter { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SearchTerm { get; set; }
    public ProductSortBy SortBy { get; set; } = ProductSortBy.Name;
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
}

