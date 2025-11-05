using Bakery.Core.Entities.People;
using Bakery.Core.Entities.Products;
using Bakery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bakery.Infrastructure.Seed;

/// <summary>
/// Comprehensive seed data provider for the bakery system
/// Provides realistic and meaningful data for development and testing
/// </summary>
public class BakerySeedDataProvider
{
    private readonly BakeryDbContext _context;
    private readonly ILogger<BakerySeedDataProvider> _logger;

    public BakerySeedDataProvider(BakeryDbContext context, ILogger<BakerySeedDataProvider> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds all data for the bakery system
    /// </summary>
    public async Task SeedAllDataAsync()
    {
        try
        {
            _logger.LogInformation("🌱 Starting comprehensive data seeding...");

            // Ensure database is created (for SQLite/SQL Server)
            if (_context.Database.IsRelational())
            {
                await _context.Database.EnsureCreatedAsync();
                _logger.LogInformation("📊 Database ensured created");
            }

            // Check if data already exists
            if (await _context.Products.AnyAsync() || await _context.Customers.AnyAsync() || await _context.Markets.AnyAsync())
            {
                _logger.LogInformation("📋 Data already exists, skipping seeding");
                return;
            }

            // Seed in order (respecting foreign keys)
            await SeedMarketsAsync();
            await SeedProductsAsync();
            await SeedCustomersAsync();

            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ Data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during data seeding");
            throw;
        }
    }

    /// <summary>
    /// Seeds market locations
    /// </summary>
    private async Task SeedMarketsAsync()
    {
        _logger.LogInformation("🏪 Seeding markets...");

        var markets = new List<Market>
        {
            new Market
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Panificio Centrale Milano",
                Address = "Via del Corso, 123",
                City = "Milano",
                State = "Lombardia",
                ZipCode = "20121",
                Email = "milano@bakery.com",
                PhoneNumber = "+39 02 8765 4321",
                OpeningTime = new TimeSpan(6, 0, 0),
                ClosingTime = new TimeSpan(20, 0, 0),
                IsOpen = true,
                CreatedAt = DateTime.UtcNow
            },
            new Market
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Dolci Tradizioni Roma",
                Address = "Piazza Navona, 45",
                City = "Roma",
                State = "Lazio",
                ZipCode = "00186",
                Email = "roma@bakery.com",
                PhoneNumber = "+39 06 8765 4321",
                OpeningTime = new TimeSpan(5, 30, 0),
                ClosingTime = new TimeSpan(21, 0, 0),
                IsOpen = true,
                CreatedAt = DateTime.UtcNow
            },
            new Market
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Forno del Sole Napoli",
                Address = "Via Toledo, 87",
                City = "Napoli",
                State = "Campania",
                ZipCode = "80134",
                Email = "napoli@bakery.com",
                PhoneNumber = "+39 081 8765 4321",
                OpeningTime = new TimeSpan(6, 0, 0),
                ClosingTime = new TimeSpan(19, 30, 0),
                IsOpen = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Markets.AddRangeAsync(markets);
        _logger.LogInformation($"📍 Added {markets.Count} markets");
    }

    /// <summary>
    /// Seeds comprehensive product catalog
    /// </summary>
    private async Task SeedProductsAsync()
    {
        _logger.LogInformation("🍞 Seeding products...");

        var milanMarketId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var romaMarketId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var napoliMarketId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        var products = new List<Product>
        {
            // PIZZAS
            new Pizza
            {
                Id = Guid.NewGuid(),
                Name = "Pizza Margherita",
                Description = "La classica pizza italiana con pomodoro, mozzarella e basilico fresco",
                Price = 8.50m,
                IsAvailable = true,
                ImageUrl = "/images/pizza-margherita.jpg",
                MarketId = milanMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                Ingredients = "Pomodoro San Marzano, Mozzarella di Bufala, Basilico fresco, Olio EVO",
                Size = "Media (30cm)",
                IsSpicy = false
            },
            new Pizza
            {
                Id = Guid.NewGuid(),
                Name = "Pizza Diavola",
                Description = "Pizza piccante con salame piccante, peperoni e mozzarella",
                Price = 10.00m,
                IsAvailable = true,
                ImageUrl = "/images/pizza-diavola.jpg",
                MarketId = milanMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                Ingredients = "Pomodoro, Mozzarella, Salame piccante, Peperoni, Peperoncino",
                Size = "Media (30cm)",
                IsSpicy = true
            },
            new Pizza
            {
                Id = Guid.NewGuid(),
                Name = "Pizza Quattro Stagioni",
                Description = "Un quarto per ogni stagione: prosciutto, funghi, carciofi e olive",
                Price = 12.00m,
                IsAvailable = true,
                ImageUrl = "/images/pizza-quattro-stagioni.jpg",
                MarketId = romaMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                Ingredients = "Pomodoro, Mozzarella, Prosciutto cotto, Funghi porcini, Carciofi, Olive nere",
                Size = "Grande (35cm)",
                IsSpicy = false
            },

            // BREADS
            new Bread
            {
                Id = Guid.NewGuid(),
                Name = "Pane Pugliese",
                Description = "Pane tradizionale pugliese con crosta croccante e mollica soffice",
                Price = 3.50m,
                IsAvailable = true,
                ImageUrl = "/images/pane-pugliese.jpg",
                MarketId = milanMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                BreadType = "Pane bianco a lievitazione naturale",
                IsGlutenFree = false,
                ShelfLifeDays = 3
            },
            new Bread
            {
                Id = Guid.NewGuid(),
                Name = "Pane Integrale ai Semi",
                Description = "Pane integrale arricchito con semi di girasole, zucca e lino",
                Price = 4.20m,
                IsAvailable = true,
                ImageUrl = "/images/pane-integrale-semi.jpg",
                MarketId = romaMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-12),
                BreadType = "Pane integrale ai cereali",
                IsGlutenFree = false,
                ShelfLifeDays = 4
            },
            new Bread
            {
                Id = Guid.NewGuid(),
                Name = "Pane Senza Glutine",
                Description = "Pane artigianale senza glutine, perfetto per celiaci",
                Price = 5.80m,
                IsAvailable = true,
                ImageUrl = "/images/pane-senza-glutine.jpg",
                MarketId = napoliMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                BreadType = "Pane senza glutine con farine alternative",
                IsGlutenFree = true,
                ShelfLifeDays = 2
            },

            // CAKES
            new Cake
            {
                Id = Guid.NewGuid(),
                Name = "Torta della Nonna",
                Description = "Classica torta italiana con crema pasticciera e pinoli",
                Price = 25.00m,
                IsAvailable = true,
                ImageUrl = "/images/torta-della-nonna.jpg",
                MarketId = milanMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                Flavor = "Crema pasticciera e limone",
                Occasion = "Famiglia e tradizione",
                ServingSize = 8
            },
            new Cake
            {
                Id = Guid.NewGuid(),
                Name = "Torta di Compleanno al Cioccolato",
                Description = "Torta al cioccolato fondente con ganache e decorazioni personalizzate",
                Price = 35.00m,
                IsAvailable = true,
                ImageUrl = "/images/torta-compleanno-cioccolato.jpg",
                MarketId = romaMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Flavor = "Cioccolato fondente",
                Occasion = "Compleanno",
                ServingSize = 12
            },
            new Cake
            {
                Id = Guid.NewGuid(),
                Name = "Torta Nuziale Elegante",
                Description = "Torta a tre piani con decorazioni in pasta di zucchero",
                Price = 120.00m,
                IsAvailable = false, // Su ordinazione
                ImageUrl = "/images/torta-nuziale.jpg",
                MarketId = napoliMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Flavor = "Vanilla e frutti di bosco",
                Occasion = "Matrimonio",
                ServingSize = 50
            },

            // PASTRIES
            new Pastrie
            {
                Id = Guid.NewGuid(),
                Name = "Cannoli Siciliani",
                Description = "Cannoli croccanti con ricotta dolce, canditi e pistacchi",
                Price = 4.50m,
                IsAvailable = true,
                ImageUrl = "/images/cannoli-siciliani.jpg",
                MarketId = napoliMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                PastrieType = "Dolce siciliano tradizionale",
                Filling = "Ricotta dolce, canditi, pistacchi",
                IsVegan = false
            },
            new Pastrie
            {
                Id = Guid.NewGuid(),
                Name = "Croissant Integrale Vegano",
                Description = "Croissant sfogliato integrale con marmellata di albicocche bio",
                Price = 2.80m,
                IsAvailable = true,
                ImageUrl = "/images/croissant-vegano.jpg",
                MarketId = milanMarketId,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                PastrieType = "Cornetto vegano",
                Filling = "Marmellata di albicocche biologiche",
                IsVegan = true
            },
            new Pastrie
            {
                Id = Guid.NewGuid(),
                Name = "Sfogliatelle Napoletane",
                Description = "Sfogliatelle croccanti ripiene di crema di semolino e canditi",
                Price = 3.20m,
                IsAvailable = true,
                ImageUrl = "/images/sfogliatelle.jpg",
                MarketId = napoliMarketId,
                CreatedAt = DateTime.UtcNow,
                PastrieType = "Dolce napoletano",
                Filling = "Crema di semolino, ricotta, canditi",
                IsVegan = false
            }
        };

        await _context.Products.AddRangeAsync(products);
        _logger.LogInformation($"🥖 Added {products.Count} products");
    }

    /// <summary>
    /// Seeds realistic customer data
    /// </summary>
    private async Task SeedCustomersAsync()
    {
        _logger.LogInformation("👥 Seeding customers...");

        var milanMarketId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var romaMarketId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var napoliMarketId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        var customers = new List<Customer>
        {
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Marco",
                LastName = "Ferrari",
                Email = "marco.ferrari@email.com",
                PhoneNumber = "+39 345 1234567",
                Address = "Via Milano 15",
                City = "Milano",
                State = "Lombardia",
                ZipCode = "20121",
                DateOfBirth = new DateTime(1985, 6, 15),
                TotalSpent = 145.50m,
                MarketId = milanMarketId,
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Elena",
                LastName = "Romano",
                Email = "elena.romano@email.com",
                PhoneNumber = "+39 347 2345678",
                Address = "Via Roma 22",
                City = "Roma",
                State = "Lazio",
                ZipCode = "00186",
                DateOfBirth = new DateTime(1992, 3, 22),
                TotalSpent = 89.30m,
                MarketId = romaMarketId,
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Francesco",
                LastName = "Conti",
                Email = "francesco.conti@email.com",
                PhoneNumber = "+39 349 3456789",
                Address = "Via Napoli 8",
                City = "Napoli",
                State = "Campania",
                ZipCode = "80134",
                DateOfBirth = new DateTime(1978, 11, 8),
                TotalSpent = 267.80m, // VIP Customer
                MarketId = napoliMarketId,
                IsVip = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-8)
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Giulia",
                LastName = "Moretti",
                Email = "giulia.moretti@email.com",
                PhoneNumber = "+39 346 4567890",
                Address = "Corso Buenos Aires 45",
                City = "Milano",
                State = "Lombardia",
                ZipCode = "20124",
                DateOfBirth = new DateTime(1990, 9, 12),
                TotalSpent = 56.40m,
                MarketId = milanMarketId,
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Alessandro",
                LastName = "Ricci",
                Email = "alessandro.ricci@email.com",
                PhoneNumber = "+39 348 5678901",
                Address = "Via del Tritone 11",
                City = "Roma",
                State = "Lazio",
                ZipCode = "00187",
                DateOfBirth = new DateTime(1973, 1, 30),
                TotalSpent = 189.60m,
                MarketId = romaMarketId,
                CreatedAt = DateTime.UtcNow.AddMonths(-10)
            }
        };

        await _context.Customers.AddRangeAsync(customers);
        _logger.LogInformation($"👤 Added {customers.Count} customers");
    }
}