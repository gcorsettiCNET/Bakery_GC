namespace Bakery_GC.Models.Local
{
    public class Bread : Product
    {
        public Guid Id { get; set; }
        public TypeOfLeavening Leavening { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}