using Bakery_GC.Models.Local.HumanResources;
using Bakery_GC.Models.Local.ObjectToSell;
using Bakery_GC.Models.Local.Orders;
using Microsoft.EntityFrameworkCore;

namespace Bakery_GC.Models.Local
{
    public class ApplicationDBContext_Local : DbContext
    {
        public ApplicationDBContext_Local() { }

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
        public DbSet<Bread> Breads { get; set; }
        public DbSet<Cake> Cakes { get; set; }
        public DbSet<Pastrie> Pastries { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Orders
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey("CustomerId")
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryOptions)
                .WithMany()
                .HasForeignKey("DeliveryOptionsId");
            // OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
            // Delivery
            modelBuilder.Entity<Delivery>()
                .HasKey(d => d.Id);

            // HumanResources
            // Market
            modelBuilder.Entity<Market>()
                .HasKey(m => m.Id);
            modelBuilder.Entity<Market>()
                .HasMany(m => m.Products)
                .WithOne(p => p.Market)
                .HasForeignKey("MarketId")
                .OnDelete(DeleteBehavior.Cascade);
            // Customer
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.Id);
            
            // ObjectToSell
            // Product 
            modelBuilder.Entity<Product>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Market)
                .WithMany(m => m.Products)
                .HasForeignKey("MarketId")
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Product>()
                .HasDiscriminator<ProductType>("ProductType")
                .HasValue<Product>(ProductType.Product)
                .HasValue<Bread>(ProductType.Bread)
                .HasValue<Cake>(ProductType.Cake)
                .HasValue<Pastrie>(ProductType.Pastrie)
                .HasValue<Pizza>(ProductType.Pizza);
            // Specific Products
            //Bread, Cake, Pastrie, Pizza
            modelBuilder.Entity<Bread>().HasKey(b => b.Id);
            modelBuilder.Entity<Cake>().HasKey(c => c.Id);
            modelBuilder.Entity<Pastrie>().HasKey(p => p.Id);
            modelBuilder.Entity<Pizza>().HasKey(p => p.Id);
        }
    }
}
