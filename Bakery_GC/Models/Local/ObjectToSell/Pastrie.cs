using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;

namespace Bakery_GC.Models.Local.ObjectToSell
{
    public class Pastrie : Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public PastryType PastryType { get; set; }
    }
}