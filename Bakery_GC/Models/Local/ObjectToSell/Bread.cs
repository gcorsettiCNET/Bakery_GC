using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;

namespace Bakery_GC.Models.Local.ObjectToSell
{
    public class Bread : Product
    {
        public TypeOfLeavening Leavening { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}