using System.Net;
using System.Reflection.Emit;

namespace Bakery_GC.Models.Local.HumanResources
{
    public class Customer : Person
    {
        public required Market Market { get; set; }
    }
}