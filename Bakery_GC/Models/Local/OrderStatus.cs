namespace Bakery_GC.Models.Local
{
    public enum OrderStatus
    {
        Unknown = 0,
        Pending,          // Order has been placed but not yet processed
        Processing,       // Order is being prepared or baked
        ReadyForPickup,   // Order is ready for customer to pick up
        Delivered,        // Order has been delivered to the customer
        Completed,        // Order has been completed and payment processed
        Cancelled,        // Order has been cancelled by the customer or bakery
        Refunded          // Order has been refunded to the customer
    }
}