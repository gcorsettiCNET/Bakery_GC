using Bakery_GC.Models.Local.HumanResources;
using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;
using System.ComponentModel.DataAnnotations;

namespace Bakery_GC.Models.Local.ObjectToSell
{
    public class Product
    {
        public Guid Id { get; set; }
        public required Market Market { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public required string ImageUrl { get; set; }
        public ProductType ProductType{ get; set; }
    }
}