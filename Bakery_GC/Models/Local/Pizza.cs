namespace Bakery_GC.Models.Local
{
    public class Pizza : Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<string> Ingredients { get; set; }
        public string ImageUrl { get; set; }
        public PizzaType PizzaType { get; set; }
    }
}