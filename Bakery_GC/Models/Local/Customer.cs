using System.Net;
using System.Reflection.Emit;

namespace Bakery_GC.Models.Local
{
    public class Customer : Person
    {
        public Market Market{ get; set; }
    }
}