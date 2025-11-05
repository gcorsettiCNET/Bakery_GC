namespace Bakery.Infrastructure.Configuration;

/// <summary>
/// Enum per i tipi di database supportati
/// </summary>
public enum DatabaseProvider
{
    InMemory,
    SqlServer,
    Sqlite
}

/// <summary>
/// Configurazione del database per l'applicazione
/// Supporta multiple provider con environment-based selection
/// </summary>
public class DatabaseConfiguration
{
    /// <summary>
    /// Provider del database da utilizzare
    /// </summary>
    public DatabaseProvider Provider { get; set; } = DatabaseProvider.InMemory;

    /// <summary>
    /// Connection string per il database
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Indica se abilitare il sensitive data logging (solo development)
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Indica se abilitare il detailed errors (solo development)
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Indica se eseguire automaticamente le migrazioni all'avvio
    /// </summary>
    public bool AutoMigrateOnStartup { get; set; } = false;

    /// <summary>
    /// Indica se eseguire il seeding dei dati all'avvio
    /// </summary>
    public bool SeedDataOnStartup { get; set; } = true;

    /// <summary>
    /// Path per il file SQLite (se provider è SQLite)
    /// </summary>
    public string SqliteFilePath { get; set; } = "bakery.db";

    /// <summary>
    /// Factory method per creare configurazione da environment
    /// </summary>
    public static DatabaseConfiguration FromEnvironment(string environment, string? connectionString = null)
    {
        return environment.ToLowerInvariant() switch
        {
            "development" => new DatabaseConfiguration
            {
                Provider = string.IsNullOrEmpty(connectionString) ? DatabaseProvider.Sqlite : DatabaseProvider.SqlServer,
                ConnectionString = connectionString ?? "Data Source=bakery-dev.db",
                EnableSensitiveDataLogging = true,
                EnableDetailedErrors = true,
                AutoMigrateOnStartup = true,
                SeedDataOnStartup = true,
                SqliteFilePath = "bakery-dev.db"
            },
            "testing" => new DatabaseConfiguration
            {
                Provider = DatabaseProvider.InMemory,
                ConnectionString = "TestDatabase",
                EnableSensitiveDataLogging = false,
                EnableDetailedErrors = true,
                AutoMigrateOnStartup = false,
                SeedDataOnStartup = true
            },
            "staging" => new DatabaseConfiguration
            {
                Provider = DatabaseProvider.SqlServer,
                ConnectionString = connectionString ?? throw new ArgumentException("Connection string required for staging"),
                EnableSensitiveDataLogging = false,
                EnableDetailedErrors = false,
                AutoMigrateOnStartup = true,
                SeedDataOnStartup = false
            },
            "production" => new DatabaseConfiguration
            {
                Provider = DatabaseProvider.SqlServer,
                ConnectionString = connectionString ?? throw new ArgumentException("Connection string required for production"),
                EnableSensitiveDataLogging = false,
                EnableDetailedErrors = false,
                AutoMigrateOnStartup = false,
                SeedDataOnStartup = false
            },
            _ => new DatabaseConfiguration
            {
                Provider = DatabaseProvider.InMemory,
                ConnectionString = "DefaultInMemoryDatabase",
                SeedDataOnStartup = true
            }
        };
    }
}