namespace Bakery.Core.Entities.Products;

/// <summary>
/// Pane con caratteristiche specifiche
/// </summary>
public class Bread : Product
{
    public string BreadType { get; set; } = string.Empty; // White, Whole Wheat, Rye, etc.
    public bool IsGlutenFree { get; set; }
    public int ShelfLifeDays { get; set; }

    public Bread()
    {
        ProductType = ProductType.Bread;
    }

    /// <summary>
    /// Business logic: Verifica se il pane è ancora fresco
    /// </summary>
    public bool IsFresh()
    {
        var daysSinceProduced = (DateTime.Now - CreatedAt).Days;
        return daysSinceProduced <= ShelfLifeDays;
    }
}