using Bakery.Core.Common;
using Bakery.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bakery.Infrastructure.Repositories;

/// <summary>
/// Implementazione dell'Unit of Work pattern
/// Coordina il lavoro di multiple repository e gestisce le transazioni
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly Dictionary<Type, object> _repositories;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed = false;

    public UnitOfWork(
        DbContext context, 
        IServiceProvider serviceProvider, 
        ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repositories = new Dictionary<Type, object>();
    }

    // ======= REPOSITORY ACCESS =======
    
    public IRepository<T, TKey> Repository<T, TKey>() where T : class
    {
        var type = typeof(T);
        
        if (_repositories.TryGetValue(type, out var existingRepo))
        {
            return (IRepository<T, TKey>)existingRepo;
        }

        // Crea nuovo repository usando Dependency Injection
        var repository = _serviceProvider.GetService<IRepository<T, TKey>>() ??
                        new Repository<T, TKey>(_context, _serviceProvider.GetRequiredService<ILogger<Repository<T, TKey>>>());
        
        _repositories[type] = repository;
        
        _logger.LogDebug("Created repository for entity type {EntityType}", typeof(T).Name);
        return repository;
    }

    // ======= TRANSACTION MANAGEMENT =======
    
    public async Task<Result> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                _logger.LogWarning("Transaction already in progress");
                return Result.Failure("Transaction already in progress");
            }

            _logger.LogDebug("Beginning new database transaction");
            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            _logger.LogInformation("Database transaction started successfully");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error beginning database transaction");
            return Result.Failure(ex);
        }
    }

    public async Task<Result> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction == null)
            {
                _logger.LogWarning("No active transaction to commit");
                return Result.Failure("No active transaction to commit");
            }

            _logger.LogDebug("Committing database transaction");
            await _currentTransaction.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Database transaction committed successfully");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing database transaction");
            await RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task<Result> RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction == null)
            {
                _logger.LogWarning("No active transaction to rollback");
                return Result.Failure("No active transaction to rollback");
            }

            _logger.LogDebug("Rolling back database transaction");
            await _currentTransaction.RollbackAsync(cancellationToken);
            
            _logger.LogInformation("Database transaction rolled back successfully");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back database transaction");
            return Result.Failure(ex);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    // ======= SAVE CHANGES =======
    
    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var pendingChanges = _context.ChangeTracker.Entries()
                .Count(e => e.State == EntityState.Added || 
                           e.State == EntityState.Modified || 
                           e.State == EntityState.Deleted);

            _logger.LogDebug("Saving {PendingChanges} pending changes to database", pendingChanges);
            
            var affectedRows = await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Successfully saved {AffectedRows} changes to database", affectedRows);
            return Result<int>.Success(affectedRows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            return Result<int>.Failure(ex);
        }
    }

    public async Task<Result<int>> SaveChangesWithTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var beginResult = await BeginTransactionAsync(cancellationToken);
            if (beginResult.IsFailure)
            {
                return Result<int>.Failure(beginResult.Error);
            }

            var saveResult = await SaveChangesAsync(cancellationToken);
            if (saveResult.IsFailure)
            {
                await RollbackTransactionAsync(cancellationToken);
                return saveResult;
            }

            var commitResult = await CommitTransactionAsync(cancellationToken);
            if (commitResult.IsFailure)
            {
                return Result<int>.Failure(commitResult.Error);
            }

            return saveResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes with transaction");
            await RollbackTransactionAsync(cancellationToken);
            return Result<int>.Failure(ex);
        }
    }

    // ======= HEALTH CHECK =======
    
    public bool HasPendingChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }

    public int GetPendingChangesCount()
    {
        return _context.ChangeTracker.Entries()
            .Count(e => e.State == EntityState.Added || 
                       e.State == EntityState.Modified || 
                       e.State == EntityState.Deleted);
    }

    // ======= PRIVATE METHODS =======
    
    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    // ======= DISPOSE PATTERN =======
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _currentTransaction?.Dispose();
            _repositories.Clear();
            _disposed = true;
            
            _logger.LogDebug("UnitOfWork disposed");
        }
    }
}