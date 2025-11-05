using AutoMapper;
using Bakery.Application.Common.Models;
using Bakery.Application.DTOs;
using Bakery.Core.Common;
using Bakery.Core.Entities;
using Bakery.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bakery.Application.Queries.Products;

/// <summary>
/// Handler for GetProductsQuery
/// </summary>
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedList<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PagedList<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting products with filters: Category={Category}, Available={Available}, Search={Search}",
                request.CategoryFilter, request.IsAvailableFilter, request.SearchTerm);

            // Get all products first
            var allProductsResult = await _productRepository.GetAllAsync();
            if (!allProductsResult.IsSuccess)
            {
                _logger.LogError("Failed to get products: {Error}", allProductsResult.Error);
                return Result<PagedList<ProductDto>>.Failure(allProductsResult.Error);
            }

            var products = allProductsResult.Value.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.CategoryFilter))
            {
                products = products.Where(p => p.ProductType.ToString().Contains(request.CategoryFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (request.IsAvailableFilter.HasValue)
            {
                products = products.Where(p => p.IsAvailable == request.IsAvailableFilter.Value);
            }

            if (request.MinPrice.HasValue)
            {
                products = products.Where(p => p.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= request.MaxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                products = products.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                    p.ProductType.ToString().ToLower().Contains(searchTerm));
            }

            // Apply sorting
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
                _ => products.OrderBy(p => p.Name)
            };

            var totalCount = products.Count();

            // Apply pagination
            var pagedProducts = products
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var productDtos = _mapper.Map<List<ProductDto>>(pagedProducts);

            var pagedResult = new PagedList<ProductDto>(
                productDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            _logger.LogDebug("Retrieved {Count} products out of {Total}", productDtos.Count, totalCount);

            return Result<PagedList<ProductDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return Result<PagedList<ProductDto>>.Failure($"Error retrieving products: {ex.Message}");
        }
    }
}