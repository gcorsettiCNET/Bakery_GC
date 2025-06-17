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

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cake> Cakes { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Bread> Breads{ get; set; }
        public DbSet<Pastry> Pastries { get; set; }
        public DbSet<PastryType> PastryTypes { get; set; }
        public DbSet<CakeType> CakeTypes { get; set; }
        public DbSet<BreadType> BreadTypes { get; set; }




    }
}
