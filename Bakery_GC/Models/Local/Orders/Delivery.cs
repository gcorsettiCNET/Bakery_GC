namespace Bakery_GC.Models.Local.Orders
{
    public class Delivery
    {
        public Guid Id { get; set; }
        public string? DeliveryAddress { get; set; } // Address where the order will be delivered
        public string? PickupLocation { get; set; } // Location for order pickup if applicable
        public DateTime? DeliveryDate { get; set; } // Date and time when the order will be delivered
        public DateTime? PickupDate { get; set; } // Date and time when the order will be ready for pickup
        public string? SpecialInstructions { get; set; } // Any special instructions for the order
        public DateTime? ShippingDate { get; set; } // Date and time when the order was shipped, if applicable
        public DateTime? EstimatedDeliveryDate { get; set; } // Estimated date and time for delivery, if applicable
        public DateTime? EstimatedPickupDate { get; set; } // Estimated date and time for pickup, if applicable
        public string? TrackingNumber { get; set; } // Tracking number for the order, if applicable
        public string? PaymentMethod { get; set; } // Payment method used for the order (e.g., Credit Card, PayPal)
        public string? OrderNotes { get; set; } // Additional notes or comments for the order
    }
}