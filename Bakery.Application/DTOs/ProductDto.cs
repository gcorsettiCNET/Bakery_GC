namespace Bakery.Application.DTOs;

/// <summary>
/// DTO for Product data transfer in CQRS operations
/// Rappresenta i dati del prodotto ottimizzati per il trasporto
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Properties specifiche per tipo di prodotto
    public string? Ingredients { get; set; }      // Pizza, general products
    public string? Size { get; set; }            // Pizza, Cake
    public bool? IsSpicy { get; set; }           // Pizza
    public string? BreadType { get; set; }       // Bread
    public bool? IsGlutenFree { get; set; }      // Bread
    public int? ShelfLifeDays { get; set; }      // Bread
    public string? Flavor { get; set; }          // Cake
    public string? Occasion { get; set; }        // Cake
    public int? ServingSize { get; set; }        // Cake
    public string? PastrieType { get; set; }     // Pastrie
    public string? Filling { get; set; }         // Pastrie
    public bool? IsVegan { get; set; }           // Pastrie
}

/// <summary>
/// DTO semplificato per liste di prodotti (performance ottimizzata)
/// </summary>
public class ProductSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// DTO per la creazione di nuovi prodotti
/// </summary>
public class CreateProductDto
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
}