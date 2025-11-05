using Bakery.Application.DTOs;
using Bakery.Application.Queries.Products;
using Bakery.Core.Common;
using Bakery.Core.Entities.Products;
using Bakery.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bakery.Application.Handlers.Products;

/// <summary>
/// Handler per la query GetProductByIdQuery
/// Implementa la logica di recupero di un prodotto specifico
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetProductByIdQuery for ProductId: {ProductId}", request.ProductId);

            var productResult = await _productRepository.GetByIdAsync(request.ProductId);
            
            if (productResult.IsFailure)
            {
                _logger.LogWarning("Product not found: {ProductId} - {Error}", request.ProductId, productResult.Error);
                return Result<ProductDto>.Failure(productResult.Error);
            }

            var product = productResult.Value;
            
            // Converti l'entity in DTO completo
            var productDto = MapToProductDto(product);

            _logger.LogInformation("Successfully retrieved product: {ProductId} - {ProductName}", product.Id, product.Name);

            return Result<ProductDto>.Success(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GetProductByIdQuery for ProductId: {ProductId}", request.ProductId);
            return Result<ProductDto>.Failure($"An error occurred while retrieving product: {ex.Message}");
        }
    }

    /// <summary>
    /// Mappa un'entity Product in ProductDto con tutte le proprietà specifiche per tipo
    /// </summary>
    private static ProductDto MapToProductDto(Product product)
    {
        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description ?? string.Empty,
            Price = product.Price,
            IsAvailable = product.IsAvailable,
            Category = product.ProductType.ToString(),
            CreatedAt = product.CreatedAt
        };

        // Mappa le proprietà specifiche per tipo usando pattern matching
        switch (product)
        {
            case Pizza pizza:
                dto.Ingredients = pizza.Ingredients;
                dto.Size = pizza.Size;
                dto.IsSpicy = pizza.IsSpicy;
                break;
                
            case Bread bread:
                dto.BreadType = bread.BreadType;
                dto.IsGlutenFree = bread.IsGlutenFree;
                dto.ShelfLifeDays = bread.ShelfLifeDays;
                break;
                
            case Cake cake:
                dto.Flavor = cake.Flavor;
                dto.Occasion = cake.Occasion;
                dto.ServingSize = cake.ServingSize;
                break;
                
            case Pastrie pastrie:
                dto.PastrieType = pastrie.PastrieType;
                dto.Filling = pastrie.Filling;
                dto.IsVegan = pastrie.IsVegan;
                break;
        }

        return dto;
    }
}