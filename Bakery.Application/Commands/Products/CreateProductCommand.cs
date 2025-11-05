using Bakery.Application.DTOs;
using Bakery.Core.Common;
using MediatR;

namespace Bakery.Application.Commands.Products;

/// <summary>
/// Command per creare un nuovo prodotto
/// Implementa il pattern CQRS per operazioni di scrittura
/// </summary>
public class CreateProductCommand : IRequest<Result<ProductDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string Category { get; set; } = string.Empty;
    
    // Properties specifiche per tipo di prodotto
    public string? Ingredients { get; set; }
    public string? Size { get; set; }
    public bool? IsSpicy { get; set; }
    public string? BreadType { get; set; }
    public bool? IsGlutenFree { get; set; }
    public int? ShelfLifeDays { get; set; }
    public string? Flavor { get; set; }
    public string? Occasion { get; set; }
    public int? ServingSize { get; set; }
    public string? PastrieType { get; set; }
    public string? Filling { get; set; }
    public bool? IsVegan { get; set; }
    
    /// <summary>
    /// Factory method per creare command da DTO
    /// </summary>
    public static CreateProductCommand FromDto(CreateProductDto dto)
    {
        return new CreateProductCommand
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            IsAvailable = dto.IsAvailable,
            Category = dto.Category,
            Ingredients = dto.Ingredients,
            Size = dto.Size,
            IsSpicy = dto.IsSpicy,
            BreadType = dto.BreadType,
            IsGlutenFree = dto.IsGlutenFree,
            ShelfLifeDays = dto.ShelfLifeDays,
            Flavor = dto.Flavor,
            Occasion = dto.Occasion,
            ServingSize = dto.ServingSize,
            PastrieType = dto.PastrieType,
            Filling = dto.Filling,
            IsVegan = dto.IsVegan
        };
    }
}