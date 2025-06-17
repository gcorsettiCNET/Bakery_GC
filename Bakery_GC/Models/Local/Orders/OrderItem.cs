using Bakery_GC.Models.Local.ObjectToSell;

namespace Bakery_GC.Models.Local.Orders
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
