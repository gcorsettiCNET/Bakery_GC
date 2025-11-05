using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;

namespace Bakery_GC.Models.Local.ObjectToSell
{
    public class Bread : Product
    {
        public TypeOfLeavening Leavening { get; set; }
        public BreadType BreadType { get; set; }
    }
}