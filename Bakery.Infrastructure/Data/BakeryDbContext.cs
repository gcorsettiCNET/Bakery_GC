using Bakery.Core.Entities.Products;
using Bakery.Core.Entities.People;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Infrastructure.Data;

/// <summary>
/// DbContext principale per l'applicazione Bakery
/// Configurato seguendo Clean Architecture principles
/// </summary>
public class BakeryDbContext : DbContext
{
    public BakeryDbContext(DbContextOptions<BakeryDbContext> options) : base(options)
    {
    }

    // ======= PRODUCT ENTITIES =======
    public DbSet<Product> Products { get; set; }
    public DbSet<Pizza> Pizzas { get; set; }
    public DbSet<Bread> Breads { get; set; }
    public DbSet<Cake> Cakes { get; set; }
    public DbSet<Pastrie> Pastries { get; set; }

    // ======= PEOPLE ENTITIES =======
    public DbSet<Market> Markets { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ======= TPH CONFIGURATION FOR PRODUCTS =======
        modelBuilder.Entity<Product>()
            .HasDiscriminator<ProductType>("ProductType")
            .HasValue<Product>(ProductType.Product)
            .HasValue<Pizza>(ProductType.Pizza)
            .HasValue<Bread>(ProductType.Bread)
            .HasValue<Cake>(ProductType.Cake)
            .HasValue<Pastrie>(ProductType.Pastrie);

        // ======= PRODUCT CONFIGURATION =======
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Description)
                .HasMaxLength(1000);

            entity.Property(p => p.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(p => p.UpdatedAt)
                .IsRequired(false);

            entity.Property(p => p.IsDeleted)
                .HasDefaultValue(false);

            entity.Property(p => p.IsAvailable)
                .HasDefaultValue(true);

            // Index per performance
            entity.HasIndex(p => p.MarketId)
                .HasDatabaseName("IX_Products_MarketId");

            entity.HasIndex(p => p.ProductType)
                .HasDatabaseName("IX_Products_ProductType");

            entity.HasIndex(p => new { p.IsAvailable, p.IsDeleted })
                .HasDatabaseName("IX_Products_Available_Deleted");

            entity.HasIndex(p => p.Price)
                .HasDatabaseName("IX_Products_Price");
        });

        // ======= PIZZA SPECIFIC CONFIGURATION =======
        modelBuilder.Entity<Pizza>(entity =>
        {
            entity.Property(p => p.Ingredients)
                .HasMaxLength(500);

            entity.Property(p => p.Size)
                .HasMaxLength(50);
        });

        // ======= BREAD SPECIFIC CONFIGURATION =======
        modelBuilder.Entity<Bread>(entity =>
        {
            entity.Property(b => b.BreadType)
                .HasMaxLength(100);

            entity.Property(b => b.ShelfLifeDays)
                .HasDefaultValue(3);
        });

        // ======= CAKE SPECIFIC CONFIGURATION =======
        modelBuilder.Entity<Cake>(entity =>
        {
            entity.Property(c => c.Flavor)
                .HasMaxLength(100);

            entity.Property(c => c.Occasion)
                .HasMaxLength(100);

            entity.Property(c => c.ServingSize)
                .HasDefaultValue(1);
        });

        // ======= PASTRIE SPECIFIC CONFIGURATION =======
        modelBuilder.Entity<Pastrie>(entity =>
        {
            entity.Property(p => p.PastrieType)
                .HasMaxLength(100);

            entity.Property(p => p.Filling)
                .HasMaxLength(200);
        });

        // ======= MARKET CONFIGURATION =======
        modelBuilder.Entity<Market>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(m => m.Address)
                .HasMaxLength(300);

            entity.Property(m => m.City)
                .HasMaxLength(100);

            entity.Property(m => m.State)
                .HasMaxLength(100);

            entity.Property(m => m.ZipCode)
                .HasMaxLength(20);

            entity.Property(m => m.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(m => m.Email)
                .HasMaxLength(200);

            entity.Property(m => m.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(m => m.IsDeleted)
                .HasDefaultValue(false);

            entity.Property(m => m.IsOpen)
                .HasDefaultValue(true);

            // Index per ricerche
            entity.HasIndex(m => m.City)
                .HasDatabaseName("IX_Markets_City");

            entity.HasIndex(m => new { m.IsOpen, m.IsDeleted })
                .HasDatabaseName("IX_Markets_Open_Deleted");
        });

        // ======= PERSON CONFIGURATION =======
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(c => c.Address)
                .HasMaxLength(300);

            entity.Property(c => c.City)
                .HasMaxLength(100);

            entity.Property(c => c.State)
                .HasMaxLength(100);

            entity.Property(c => c.ZipCode)
                .HasMaxLength(20);

            entity.Property(c => c.TotalSpent)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            entity.Property(c => c.IsVip)
                .HasDefaultValue(false);

            entity.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            // Index per performance
            entity.HasIndex(c => c.Email)
                .IsUnique()
                .HasDatabaseName("IX_Customers_Email");

            entity.HasIndex(c => c.MarketId)
                .HasDatabaseName("IX_Customers_MarketId");

            entity.HasIndex(c => c.IsVip)
                .HasDatabaseName("IX_Customers_VIP");

            entity.HasIndex(c => c.TotalSpent)
                .HasDatabaseName("IX_Customers_TotalSpent");
        });

        // ======= RELATIONSHIPS =======
        
        // Product -> Market (Many-to-One)
        modelBuilder.Entity<Product>()
            .HasOne<Market>()
            .WithMany()
            .HasForeignKey(p => p.MarketId)
            .OnDelete(DeleteBehavior.Restrict);

        // Customer -> Market (Many-to-One)
        modelBuilder.Entity<Customer>()
            .HasOne<Market>()
            .WithMany()
            .HasForeignKey(c => c.MarketId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}