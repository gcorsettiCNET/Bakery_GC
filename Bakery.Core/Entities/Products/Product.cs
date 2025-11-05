namespace Bakery.Core.Entities.Products;

/// <summary>
/// Enum per i tipi di prodotto - TPH Discriminator
/// </summary>
public enum ProductType
{
    Product = 0,
    Pizza = 1,
    Bread = 2,
    Cake = 3,
    Pastrie = 4
}

/// <summary>
/// Entity base per tutti i prodotti della panetteria
/// Implementa Table Per Hierarchy (TPH) pattern
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public ProductType ProductType { get; set; }
    
    // Navigation properties
    public Guid MarketId { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Business logic: Calcola prezzo con sconto
    /// </summary>
    public decimal CalculateDiscountedPrice(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 0 and 100");
            
        return Price * (1 - discountPercentage / 100);
    }

    /// <summary>
    /// Business logic: Verifica se prodotto è ordinabile
    /// </summary>
    public bool CanBeOrdered()
    {
        return IsAvailable && !IsDeleted && Price > 0;
    }
}