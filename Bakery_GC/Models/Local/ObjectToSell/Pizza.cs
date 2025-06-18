using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;

namespace Bakery_GC.Models.Local.ObjectToSell
{
    public class Pizza : Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<string> Ingredients { get; set; }
        public string ImageUrl { get; set; }
        public PizzaType PizzaType { get; set; }
    }
}