using Bakery_GC.Models.Local.HumanResources;
using Bakery_GC.Models.Local.ObjectToSell;
using Bakery_GC.Models.Local.Orders;
using Microsoft.EntityFrameworkCore;

namespace Bakery_GC.Models.Local
{
    public class ApplicationDBContext_Local : DbContext
    {
        public ApplicationDBContext_Local()
        {
        }

        public ApplicationDBContext_Local(DbContextOptions<ApplicationDBContext_Local> options)
            : base(options)
        {
        }
        // Orders
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        // HumanResources
        public DbSet<Market> Markets { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        // ObjectToSell
        public DbSet<Bread> Breads{ get; set; }
        public DbSet<Cake> Cakes { get; set; }
        public DbSet<Pastrie> Pastries { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }

        // Orders

    }
}
