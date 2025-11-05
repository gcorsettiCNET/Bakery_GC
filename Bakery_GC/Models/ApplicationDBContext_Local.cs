using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Bakery_GC.Models.Local.HumanResources;
using Bakery_GC.Models.Local.ObjectToSell;
using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;
using Bakery_GC.Models.Local.Orders; // ensure entity namespaces

namespace Bakery_GC.Models
{
    public class ApplicationDBContext_Local : DbContext
    {
        public ApplicationDBContext_Local() { }
        public ApplicationDBContext_Local(DbContextOptions<ApplicationDBContext_Local> options) : base(options) { }

        // Orders (placeholders if you have these entity classes elsewhere)
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }

        // HumanResources
        public DbSet<Market> Markets { get; set; }
        public DbSet<Customer> Customers { get; set; }

        // Products (TPH root)
        public DbSet<Product> Products { get; set; }
        public DbSet<Bread> Breads { get; set; }
        public DbSet<Cake> Cakes { get; set; }
        public DbSet<Pastrie> Pastries { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --------- TPH Discriminator Configuration ----------
            modelBuilder.Entity<Product>()
                .HasDiscriminator<ProductType>("ProductType")
                .HasValue<Product>(ProductType.Product)
                .HasValue<Pizza>(ProductType.Pizza)
                .HasValue<Pastrie>(ProductType.Pastrie)
                .HasValue<Bread>(ProductType.Bread)
                .HasValue<Cake>(ProductType.Cake);

            // Optional: basic property configurations
            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            // --------- Market Relationship (FK recommended) ----------
            // Add a shadow FK column if not explicitly on the model
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Market)
                .WithMany(m => m.Products)
                .OnDelete(DeleteBehavior.Restrict);

            // --------- Ingredients Collections (Pizza & Pastrie) ----------
            // We serialize List<string> as JSON into NVARCHAR(MAX)
            var ingredientsConverter = new ValueConverter<List<string>, string>(
                to => JsonSerializer.Serialize(to ?? new List<string>(), (JsonSerializerOptions?)null),
                from => string.IsNullOrWhiteSpace(from)
                    ? new List<string>()
                    : (JsonSerializer.Deserialize<List<string>>(from, (JsonSerializerOptions?)null) ?? new List<string>())
            );

            modelBuilder.Entity<Pizza>()
                .Property(p => p.Ingredients)
                .HasConversion(ingredientsConverter)
                .HasColumnName("Pizza_Ingredients");

            modelBuilder.Entity<Pastrie>()
                .Property(p => p.Ingredients)
                .HasConversion(ingredientsConverter)
                .HasColumnName("Pastrie_Ingredients");

            // Make columns nullable (EF will allow null) but we keep empty list in code
            modelBuilder.Entity<Pizza>()
                .Property<string>("Pizza_Ingredients")
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Pastrie>()
                .Property<string>("Pastrie_Ingredients")
                .HasColumnType("nvarchar(max)");

            // Index suggestions (optional)
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name);

            modelBuilder.Entity<Product>()
                .HasIndex("ProductType");
        }
    }
}