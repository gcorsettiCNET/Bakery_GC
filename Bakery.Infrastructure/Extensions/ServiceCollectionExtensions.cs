using Bakery.Core.Interfaces;
using Bakery.Core.Interfaces.Repositories;
using Bakery.Core.Entities.People;
using Bakery.Core.Entities.Products;
using Bakery.Infrastructure.Configuration;
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
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var connectionString = configuration.GetConnectionString("BakeryContext");
        var dbConfig = DatabaseConfiguration.FromEnvironment(environment, connectionString);
        
        // Registra la configurazione per DI
        services.AddSingleton(dbConfig);
        
        // Configura DbContext based on provider
        services.AddDbContext<BakeryDbContext>(options =>
        {
            ConfigureDatabase(options, dbConfig);
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
    /// Configura il database con migrazioni automatiche e seeding basato su configurazione
    /// </summary>
    public static async Task<IServiceProvider> InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BakeryDbContext>();
        var dbConfig = scope.ServiceProvider.GetRequiredService<DatabaseConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BakeryDbContext>>();
        
        try
        {
            // Apply migrations if configured
            if (dbConfig.AutoMigrateOnStartup && context.Database.IsRelational())
            {
                logger.LogInformation("🔄 Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("✅ Database migrations applied successfully");
            }
            else
            {
                // Ensure database is created for non-relational (InMemory)
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("📊 Database ensured created");
            }

            // Seed data if configured
            if (dbConfig.SeedDataOnStartup)
            {
                logger.LogInformation("🌱 Starting data seeding...");
                var seedProvider = new Seed.BakerySeedDataProvider(context, 
                    scope.ServiceProvider.GetRequiredService<ILogger<Seed.BakerySeedDataProvider>>());
                await seedProvider.SeedAllDataAsync();
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
    /// Configura il database provider basato sulla configurazione
    /// </summary>
    private static void ConfigureDatabase(DbContextOptionsBuilder options, DatabaseConfiguration dbConfig)
    {
        switch (dbConfig.Provider)
        {
            case DatabaseProvider.InMemory:
                options.UseInMemoryDatabase(dbConfig.ConnectionString);
                break;
                
            case DatabaseProvider.SqlServer:
                options.UseSqlServer(dbConfig.ConnectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                    
                    sqlOptions.CommandTimeout(30);
                    sqlOptions.MigrationsAssembly("Bakery.Infrastructure");
                });
                break;
                
            case DatabaseProvider.Sqlite:
                options.UseSqlite(dbConfig.ConnectionString, sqliteOptions =>
                {
                    sqliteOptions.MigrationsAssembly("Bakery.Infrastructure");
                });
                break;
                
            default:
                throw new ArgumentException($"Unsupported database provider: {dbConfig.Provider}");
        }

        // Configurazioni comuni
        if (dbConfig.EnableSensitiveDataLogging)
            options.EnableSensitiveDataLogging();
            
        if (dbConfig.EnableDetailedErrors)
            options.EnableDetailedErrors();
    }
}