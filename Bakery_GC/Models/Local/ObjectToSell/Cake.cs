using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;

namespace Bakery_GC.Models.Local.ObjectToSell
{
    public class Cake : Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public CakeType CakeType { get; set; }
    }
}