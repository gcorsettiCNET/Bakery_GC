using Bakery.Core.Interfaces;
using Bakery.Core.Interfaces.Repositories;
using Bakery.Core.Entities.People;
using Bakery.Core.Entities.Products;
using Bakery.Infrastructure.Data;
using Bakery.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bakery.Infrastructure.Extensions;

/// <summary>
/// Extension methods per configurare i servizi dell'Infrastructure layer
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra tutti i servizi dell'Infrastructure layer
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ======= DATABASE CONFIGURATION =======
        services.AddDbContext<BakeryDbContext>(options =>
        {
            // Use InMemory database for testing purposes
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.UseInMemoryDatabase("BakeryTestDb");
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
            else
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("BakeryContext"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                        
                        sqlOptions.CommandTimeout(30);
                    });
            }
        });

        // ======= REPOSITORY REGISTRATION =======
        
        // Generic Repository
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        
        // Specific Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // DbContext as interface for repositories
        services.AddScoped<DbContext>(provider => provider.GetService<BakeryDbContext>()!);

        return services;
    }

    /// <summary>
    /// Configura il database con migrazioni automatiche in Development
    /// </summary>
    public static async Task<IServiceProvider> InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BakeryDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BakeryDbContext>>();
        
        try
        {
            // Ensure database is created (for InMemory or real databases)
            await context.Database.EnsureCreatedAsync();
            
            // Apply migrations only for real databases, not InMemory
            if (!context.Database.IsInMemory())
            {
                await context.Database.MigrateAsync();
            }
            
            // Seed data in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                await SeedDataAsync(context);
                logger.LogInformation("✅ Database seeded successfully with test data");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ An error occurred while initializing the database");
            throw;
        }

        return services;
    }

    /// <summary>
    /// Seed data per testing
    /// </summary>
    private static async Task SeedDataAsync(BakeryDbContext context)
    {
        // Check se già esistono dati
        if (await context.Markets.AnyAsync())
        {
            return; // Database già popolato
        }

        // Seed Markets
        var markets = new[]
        {
            new Market 
            { 
                Id = Guid.NewGuid(),
                Name = "Bakery Central Milano",
                Address = "Via Roma 123",
                City = "Milano",
                State = "Lombardia",
                ZipCode = "20121",
                PhoneNumber = "+39 02 1234567",
                Email = "milano@bakery.com",
                OpeningTime = new TimeSpan(7, 0, 0),
                ClosingTime = new TimeSpan(20, 0, 0),
                IsOpen = true,
                CreatedAt = DateTime.UtcNow
            },
            new Market 
            { 
                Id = Guid.NewGuid(),
                Name = "Bakery Roma Centro",
                Address = "Via del Corso 456",
                City = "Roma",
                State = "Lazio",
                ZipCode = "00187",
                PhoneNumber = "+39 06 7654321",
                Email = "roma@bakery.com",
                OpeningTime = new TimeSpan(6, 30, 0),
                ClosingTime = new TimeSpan(21, 0, 0),
                IsOpen = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Markets.AddRangeAsync(markets);
        await context.SaveChangesAsync();

        var milanoMarket = markets[0];
        var romaMarket = markets[1];

        // Seed Products
        var products = new Product[]
        {
            // Pizze
            new Pizza 
            { 
                Id = Guid.NewGuid(),
                Name = "Pizza Margherita",
                Description = "Pizza classica con pomodoro, mozzarella e basilico",
                Price = 8.50m,
                IsAvailable = true,
                ImageUrl = "/images/pizza-margherita.jpg",
                MarketId = milanoMarket.Id,
                Ingredients = "Pomodoro, Mozzarella, Basilico",
                Size = "Medium",
                IsSpicy = false,
                CreatedAt = DateTime.UtcNow
            },
            new Pizza 
            { 
                Id = Guid.NewGuid(),
                Name = "Pizza Diavola",
                Description = "Pizza piccante con salame piccante",
                Price = 10.00m,
                IsAvailable = true,
                ImageUrl = "/images/pizza-diavola.jpg",
                MarketId = romaMarket.Id,
                Ingredients = "Pomodoro, Mozzarella, Salame Piccante",
                Size = "Large",
                IsSpicy = true,
                CreatedAt = DateTime.UtcNow
            },

            // Pane
            new Bread 
            { 
                Id = Guid.NewGuid(),
                Name = "Pane Integrale",
                Description = "Pane integrale fresco fatto in casa",
                Price = 3.50m,
                IsAvailable = true,
                ImageUrl = "/images/pane-integrale.jpg",
                MarketId = milanoMarket.Id,
                BreadType = "Integrale",
                IsGlutenFree = false,
                ShelfLifeDays = 3,
                CreatedAt = DateTime.UtcNow
            },

            // Torte
            new Cake 
            { 
                Id = Guid.NewGuid(),
                Name = "Torta al Cioccolato",
                Description = "Deliziosa torta al cioccolato per ogni occasione",
                Price = 25.00m,
                IsAvailable = true,
                ImageUrl = "/images/torta-cioccolato.jpg",
                MarketId = romaMarket.Id,
                Flavor = "Cioccolato",
                Occasion = "Compleanno",
                IsCustomizable = true,
                ServingSize = 8,
                CreatedAt = DateTime.UtcNow
            },

            // Pasticceria
            new Pastrie 
            { 
                Id = Guid.NewGuid(),
                Name = "Cornetto alla Crema",
                Description = "Cornetto fresco ripieno di crema pasticcera",
                Price = 2.20m,
                IsAvailable = true,
                ImageUrl = "/images/cornetto-crema.jpg",
                MarketId = milanoMarket.Id,
                PastrieType = "Cornetto",
                IsFilled = true,
                Filling = "Crema Pasticcera",
                IsVegan = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        // Seed Customers
        var customers = new[]
        {
            new Customer 
            { 
                Id = Guid.NewGuid(),
                FirstName = "Marco",
                LastName = "Rossi",
                Email = "marco.rossi@email.com",
                PhoneNumber = "+39 333 1234567",
                Address = "Via Milano 10",
                City = "Milano",
                State = "Lombardia",
                ZipCode = "20121",
                DateOfBirth = new DateTime(1985, 5, 15),
                MarketId = milanoMarket.Id,
                TotalSpent = 150.75m,
                IsVip = false,
                CreatedAt = DateTime.UtcNow
            },
            new Customer 
            { 
                Id = Guid.NewGuid(),
                FirstName = "Sofia",
                LastName = "Bianchi",
                Email = "sofia.bianchi@email.com",
                PhoneNumber = "+39 333 7654321",
                Address = "Via Roma 25",
                City = "Roma",
                State = "Lazio",
                ZipCode = "00187",
                DateOfBirth = new DateTime(1990, 8, 22),
                MarketId = romaMarket.Id,
                TotalSpent = 320.50m,
                IsVip = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Customers.AddRangeAsync(customers);
        await context.SaveChangesAsync();
    }
}