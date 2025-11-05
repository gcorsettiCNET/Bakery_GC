namespace Bakery.Core.Entities.Products;

/// <summary>
/// Torta con occasioni specifiche
/// </summary>
public class Cake : Product
{
    public string Flavor { get; set; } = string.Empty; // Chocolate, Vanilla, Strawberry
    public string Occasion { get; set; } = string.Empty; // Birthday, Wedding, Anniversary
    public bool IsCustomizable { get; set; }
    public int ServingSize { get; set; }

    public Cake()
    {
        ProductType = ProductType.Cake;
    }

    /// <summary>
    /// Business logic: Calcola prezzo per persona
    /// </summary>
    public decimal GetPricePerPerson()
    {
        return ServingSize > 0 ? Price / ServingSize : 0;
    }
}