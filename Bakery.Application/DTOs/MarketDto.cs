namespace Bakery.Application.DTOs;

/// <summary>
/// DTO for Market data transfer
/// </summary>
public class MarketDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime OpeningDate { get; set; }
    public bool IsActive { get; set; }
}
