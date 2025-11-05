using Bakery.Application.DTOs;
using Bakery.Application.Queries.Products;
using Bakery.Core.Common;
using Bakery.Core.Entities.Products;
using Bakery.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bakery.Application.Handlers.Products;

/// <summary>
/// Handler per la query GetAllProductsQuery
/// Implementa la logica di recupero di tutti i prodotti con filtri e ordinamento
/// </summary>
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductSummaryDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(
        IProductRepository productRepository,
        ILogger<GetAllProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ProductSummaryDto>>> Handle(
        GetAllProductsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetAllProductsQuery with filters: Category={Category}, Available={IsAvailable}, SortBy={SortBy}",
                request.CategoryFilter, request.IsAvailableFilter, request.SortBy);

            // Recupera tutti i prodotti tramite repository
            var productsResult = await _productRepository.GetAllAsync();
            
            if (productsResult.IsFailure)
            {
                _logger.LogError("Failed to retrieve products: {Error}", productsResult.Error);
                return Result<IEnumerable<ProductSummaryDto>>.Failure(productsResult.Error);
            }

            var products = productsResult.Value;
            
            // Applica filtri
            if (!string.IsNullOrEmpty(request.CategoryFilter))
            {
                products = products.Where(p => p.ProductType.ToString().Contains(request.CategoryFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (request.IsAvailableFilter.HasValue)
            {
                products = products.Where(p => p.IsAvailable == request.IsAvailableFilter.Value);
            }

            // Applica ordinamento
            products = request.SortBy switch
            {
                ProductSortBy.Name => request.SortDirection == SortDirection.Ascending 
                    ? products.OrderBy(p => p.Name) 
                    : products.OrderByDescending(p => p.Name),
                ProductSortBy.Price => request.SortDirection == SortDirection.Ascending 
                    ? products.OrderBy(p => p.Price) 
                    : products.OrderByDescending(p => p.Price),
                ProductSortBy.Category => request.SortDirection == SortDirection.Ascending 
                    ? products.OrderBy(p => p.ProductType) 
                    : products.OrderByDescending(p => p.ProductType),
                ProductSortBy.CreatedAt => request.SortDirection == SortDirection.Ascending 
                    ? products.OrderBy(p => p.CreatedAt) 
                    : products.OrderByDescending(p => p.CreatedAt),
                _ => products.OrderBy(p => p.Name)
            };

            // Converti in DTOs ottimizzati per la response
            var productDtos = products.Select(p => new ProductSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                IsAvailable = p.IsAvailable,
                Category = p.ProductType.ToString()
            });

            _logger.LogInformation("Successfully retrieved {Count} products", productDtos.Count());

            return Result<IEnumerable<ProductSummaryDto>>.Success(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GetAllProductsQuery");
            return Result<IEnumerable<ProductSummaryDto>>.Failure($"An error occurred while retrieving products: {ex.Message}");
        }
    }
}