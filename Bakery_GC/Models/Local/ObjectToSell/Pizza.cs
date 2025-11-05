using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;

namespace Bakery_GC.Models.Local.ObjectToSell
{
    public class Pizza : Product
    {
        public List<string> Ingredients { get; set; } = new List<string>();
        public PizzaType PizzaType { get; set; }
    }
}