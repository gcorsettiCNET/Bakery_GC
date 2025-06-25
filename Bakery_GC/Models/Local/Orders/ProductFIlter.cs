using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;
using Bakery_GC.Models.Local.ObjectToSell;

public class ProductFilter
{
    public ProductType? ProductType { get; set; }
    public PizzaType? PizzaType { get; set; }
    public BreadType? BreadType { get; set; }
    public CakeType? CakeType { get; set; }
    public PastryType? PastryType { get; set; }
    public Guid? MarketId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsAvailable { get; set; }
    public string? SearchTerm { get; set; }
}