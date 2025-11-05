namespace Bakery.Core.Entities.Products;

/// <summary>
/// Pasticceria dolce
/// </summary>
public class Pastrie : Product
{
    public string PastrieType { get; set; } = string.Empty; // Croissant, Danish, Muffin
    public bool IsFilled { get; set; }
    public string? Filling { get; set; }
    public bool IsVegan { get; set; }

    public Pastrie()
    {
        ProductType = ProductType.Pastrie;
    }

    /// <summary>
    /// Business logic: Verifica se è adatto per vegani
    /// </summary>
    public bool IsSuitableForVegans()
    {
        return IsVegan;
    }
}