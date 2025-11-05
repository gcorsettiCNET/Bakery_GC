namespace Bakery.Core.Entities.People;

/// <summary>
/// Negozio/Punto vendita
/// </summary>
public class Market
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
    public bool IsOpen { get; set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Business logic: Verifica se il negozio è aperto ora
    /// </summary>
    public bool IsCurrentlyOpen()
    {
        if (!IsOpen) return false;
        
        var now = DateTime.Now.TimeOfDay;
        return now >= OpeningTime && now <= ClosingTime;
    }

    /// <summary>
    /// Business logic: Calcola ore di apertura
    /// </summary>
    public double GetDailyOpeningHours()
    {
        return (ClosingTime - OpeningTime).TotalHours;
    }
}