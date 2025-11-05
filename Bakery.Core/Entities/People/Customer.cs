namespace Bakery.Core.Entities.People;

/// <summary>
/// Cliente del negozio
/// </summary>
public class Customer : Person
{
    public Guid MarketId { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public decimal TotalSpent { get; set; }
    public bool IsVip { get; set; }

    /// <summary>
    /// Business logic: Verifica se è un cliente abituale
    /// </summary>
    public bool IsRegularCustomer()
    {
        return LastOrderDate.HasValue && 
               (DateTime.Now - LastOrderDate.Value).Days <= 30 &&
               TotalSpent > 100;
    }

    /// <summary>
    /// Business logic: Calcola sconto VIP
    /// </summary>
    public decimal GetVipDiscountPercentage()
    {
        if (!IsVip) return 0;
        
        return TotalSpent switch
        {
            >= 1000 => 15m,
            >= 500 => 10m,
            >= 250 => 5m,
            _ => 0m
        };
    }
}