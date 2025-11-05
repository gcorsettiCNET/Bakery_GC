namespace Bakery.Application.DTOs;

/// <summary>
/// DTO for Customer data transfer in CQRS operations
/// </summary>
public class CustomerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public decimal TotalSpent { get; set; }
    public bool IsVip { get; set; }
    public decimal VipDiscountPercentage { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO semplificato per liste di clienti
/// </summary>
public class CustomerSummaryDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public bool IsVip { get; set; }
}

/// <summary>
/// DTO per la creazione di nuovi clienti
/// </summary>
public class CreateCustomerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
}