namespace Bakery.Application.DTOs;

/// <summary>
/// DTO for Order data transfer
/// </summary>
public class OrderDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO for OrderItem data transfer
/// </summary>
public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
