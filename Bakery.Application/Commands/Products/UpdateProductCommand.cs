using Bakery.Application.DTOs;
using Bakery.Core.Common;
using MediatR;

namespace Bakery.Application.Commands.Products;

/// <summary>
/// Command per aggiornare un prodotto esistente
/// </summary>
public class UpdateProductCommand : IRequest<Result<ProductDto>>
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    
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
}