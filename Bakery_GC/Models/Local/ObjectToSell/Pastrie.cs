using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;

namespace Bakery_GC.Models.Local.ObjectToSell
{
    public class Pastrie : Product
    {
        public PastryType PastryType { get; set; }
        public List<string> Ingredients { get; set; } = new List<string>();
    }
}