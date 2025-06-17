using Bakery_GC.Models.Local.HumanResources;

namespace Bakery_GC.Models.Local.Orders
{
    public class Order
    {
        public Guid Id { get; set; }
        public Customer Customer { get; set; } // The customer who placed the order
        public List<OrderItem> OrderItems { get; set; } // List of products in the order
        public DateTime OrderDate { get; set; } // Date and time when the order was placed
        public Delivery DeliveryOptions { get; set; }
        public decimal TotalAmount { get; set; } // Total amount for the order
        public OrderStatus Status { get; set; } // Status of the order (e.g., Pending, Completed, Cancelled)
        public bool ToShip { get; set; }
    }
}