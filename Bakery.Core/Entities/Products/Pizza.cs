namespace Bakery.Core.Entities.Products;

/// <summary>
/// Pizza con ingredienti specifici
/// </summary>
public class Pizza : Product
{
    public string Ingredients { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty; // Small, Medium, Large
    public bool IsSpicy { get; set; }

    public Pizza()
    {
        ProductType = ProductType.Pizza;
    }

    /// <summary>
    /// Business logic: Calcola tempo di preparazione in base alla dimensione
    /// </summary>
    public int GetPreparationTimeMinutes()
    {
        return Size.ToLower() switch
        {
            "small" => 15,
            "medium" => 20,
            "large" => 25,
            _ => 20
        };
    }
}