using Bakery.Application.Common.Models;
using Bakery.Application.DTOs;
using Bakery.Application.Queries.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bakery_GC.Pages;

/// <summary>
/// Products page using Clean Architecture + CQRS pattern
/// </summary>
public class ProductsModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsModel> _logger;

    public ProductsModel(IMediator mediator, ILogger<ProductsModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // Binding properties for filtering and pagination
    [BindProperty(SupportsGet = true)]
    public string? CategoryFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsAvailableFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public ProductSortBy SortBy { get; set; } = ProductSortBy.Name;

    [BindProperty(SupportsGet = true)]
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 12;

    [BindProperty(SupportsGet = true)]
    public decimal? MinPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MaxPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    // Results
    public IReadOnlyList<ProductDto> Products { get; private set; } = Array.Empty<ProductDto>();
    public int TotalCount { get; private set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public bool HasActiveFilters =>
        !string.IsNullOrWhiteSpace(CategoryFilter) ||
        IsAvailableFilter.HasValue ||
        MinPrice.HasValue ||
        MaxPrice.HasValue ||
        !string.IsNullOrWhiteSpace(SearchTerm);

    public async Task OnGetAsync()
    {
        try
        {
            var query = new GetProductsQuery
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                CategoryFilter = CategoryFilter,
                IsAvailableFilter = IsAvailableFilter,
                MinPrice = MinPrice,
                MaxPrice = MaxPrice,
                SearchTerm = SearchTerm,
                SortBy = SortBy,
                SortDirection = SortDirection
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                Products = result.Value.Items;
                TotalCount = result.Value.TotalCount;
            }
            else
            {
                _logger.LogError("Failed to retrieve products: {Error}", result.Error);
                Products = Array.Empty<ProductDto>();
                TotalCount = 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products page");
            Products = Array.Empty<ProductDto>();
            TotalCount = 0;
        }
    }

    public IActionResult OnPostAddToCart(int productId)
    {
        // Redirect al controller per gestire il carrello
        return RedirectToAction("AddToCart", "Home", new { productId });
    }
}
