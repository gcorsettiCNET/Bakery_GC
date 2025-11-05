using Bakery.Core.Common;
using Bakery.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Bakery.Infrastructure.Repositories;

/// <summary>
/// Implementazione generica del Repository Pattern
/// Fornisce operazioni CRUD base per tutte le entità
/// </summary>
public class Repository<T, TKey> : IRepository<T, TKey> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<Repository<T, TKey>> _logger;

    public Repository(DbContext context, ILogger<Repository<T, TKey>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbSet = _context.Set<T>();
    }

    // ======= QUERY METHODS =======
    
    public virtual async Task<Result<T>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entity {EntityType} with ID {Id}", typeof(T).Name, id);
            
            var entity = await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
            
            if (entity == null)
            {
                _logger.LogWarning("Entity {EntityType} with ID {Id} not found", typeof(T).Name, id);
                return Result<T>.Failure($"Entity {typeof(T).Name} with ID {id} not found");
            }

            _logger.LogDebug("Successfully retrieved entity {EntityType} with ID {Id}", typeof(T).Name, id);
            return Result<T>.Success(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity {EntityType} with ID {Id}", typeof(T).Name, id);
            return Result<T>.Failure(ex);
        }
    }

    public virtual async Task<Result<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all entities of type {EntityType}", typeof(T).Name);
            
            var entities = await _dbSet.ToListAsync(cancellationToken);
            
            _logger.LogDebug("Successfully retrieved {Count} entities of type {EntityType}", entities.Count, typeof(T).Name);
            return Result<IEnumerable<T>>.Success(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities of type {EntityType}", typeof(T).Name);
            return Result<IEnumerable<T>>.Failure(ex);
        }
    }

    public virtual async Task<Result<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Finding entities of type {EntityType} with predicate", typeof(T).Name);
            
            var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
            
            _logger.LogDebug("Found {Count} entities of type {EntityType}", entities.Count, typeof(T).Name);
            return Result<IEnumerable<T>>.Success(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding entities of type {EntityType}", typeof(T).Name);
            return Result<IEnumerable<T>>.Failure(ex);
        }
    }

    public virtual async Task<Result<T>> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting first entity of type {EntityType} with predicate", typeof(T).Name);
            
            var entity = await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
            
            if (entity == null)
            {
                _logger.LogWarning("No entity of type {EntityType} found with the given predicate", typeof(T).Name);
                return Result<T>.Failure($"No entity of type {typeof(T).Name} found with the given predicate");
            }

            _logger.LogDebug("Successfully retrieved first entity of type {EntityType}", typeof(T).Name);
            return Result<T>.Success(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting first entity of type {EntityType}", typeof(T).Name);
            return Result<T>.Failure(ex);
        }
    }

    public virtual async Task<Result<bool>> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if any entity of type {EntityType} exists with predicate", typeof(T).Name);
            
            var exists = await _dbSet.AnyAsync(predicate, cancellationToken);
            
            _logger.LogDebug("Entity existence check for {EntityType}: {Exists}", typeof(T).Name, exists);
            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking entity existence for type {EntityType}", typeof(T).Name);
            return Result<bool>.Failure(ex);
        }
    }

    public virtual async Task<Result<int>> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Counting entities of type {EntityType}", typeof(T).Name);
            
            var count = predicate == null 
                ? await _dbSet.CountAsync(cancellationToken)
                : await _dbSet.CountAsync(predicate, cancellationToken);
            
            _logger.LogDebug("Entity count for {EntityType}: {Count}", typeof(T).Name, count);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting entities of type {EntityType}", typeof(T).Name);
            return Result<int>.Failure(ex);
        }
    }

    // ======= COMMAND METHODS =======
    
    public virtual async Task<Result<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Adding new entity of type {EntityType}", typeof(T).Name);
            
            var entry = await _dbSet.AddAsync(entity, cancellationToken);
            
            _logger.LogDebug("Successfully added entity of type {EntityType}", typeof(T).Name);
            return Result<T>.Success(entry.Entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(T).Name);
            return Result<T>.Failure(ex);
        }
    }

    public virtual async Task<Result<IEnumerable<T>>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            var entityList = entities.ToList();
            _logger.LogDebug("Adding {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);
            
            await _dbSet.AddRangeAsync(entityList, cancellationToken);
            
            _logger.LogDebug("Successfully added {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);
            return Result<IEnumerable<T>>.Success(entityList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding multiple entities of type {EntityType}", typeof(T).Name);
            return Result<IEnumerable<T>>.Failure(ex);
        }
    }

    public virtual Result<T> Update(T entity)
    {
        try
        {
            _logger.LogDebug("Updating entity of type {EntityType}", typeof(T).Name);
            
            var entry = _dbSet.Update(entity);
            
            _logger.LogDebug("Successfully updated entity of type {EntityType}", typeof(T).Name);
            return Result<T>.Success(entry.Entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType}", typeof(T).Name);
            return Result<T>.Failure(ex);
        }
    }

    public virtual Result<IEnumerable<T>> UpdateRange(IEnumerable<T> entities)
    {
        try
        {
            var entityList = entities.ToList();
            _logger.LogDebug("Updating {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);
            
            _dbSet.UpdateRange(entityList);
            
            _logger.LogDebug("Successfully updated {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);
            return Result<IEnumerable<T>>.Success(entityList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating multiple entities of type {EntityType}", typeof(T).Name);
            return Result<IEnumerable<T>>.Failure(ex);
        }
    }

    public virtual async Task<Result> RemoveAsync(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Removing entity {EntityType} with ID {Id}", typeof(T).Name, id);
            
            var entity = await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Cannot remove entity {EntityType} with ID {Id} - not found", typeof(T).Name, id);
                return Result.Failure($"Entity {typeof(T).Name} with ID {id} not found");
            }

            _dbSet.Remove(entity);
            
            _logger.LogDebug("Successfully removed entity {EntityType} with ID {Id}", typeof(T).Name, id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing entity {EntityType} with ID {Id}", typeof(T).Name, id);
            return Result.Failure(ex);
        }
    }

    public virtual Result Remove(T entity)
    {
        try
        {
            _logger.LogDebug("Removing entity of type {EntityType}", typeof(T).Name);
            
            _dbSet.Remove(entity);
            
            _logger.LogDebug("Successfully removed entity of type {EntityType}", typeof(T).Name);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing entity of type {EntityType}", typeof(T).Name);
            return Result.Failure(ex);
        }
    }

    public virtual Result RemoveRange(IEnumerable<T> entities)
    {
        try
        {
            var entityList = entities.ToList();
            _logger.LogDebug("Removing {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);
            
            _dbSet.RemoveRange(entityList);
            
            _logger.LogDebug("Successfully removed {Count} entities of type {EntityType}", entityList.Count, typeof(T).Name);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing multiple entities of type {EntityType}", typeof(T).Name);
            return Result.Failure(ex);
        }
    }

    // ======= SOFT DELETE =======
    
    public virtual async Task<Result> SoftDeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Soft deleting entity {EntityType} with ID {Id}", typeof(T).Name, id);
            
            var entity = await _dbSet.FindAsync(new object[] { id! }, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Cannot soft delete entity {EntityType} with ID {Id} - not found", typeof(T).Name, id);
                return Result.Failure($"Entity {typeof(T).Name} with ID {id} not found");
            }

            // Verifica se l'entità ha proprietà IsDeleted
            var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
            if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool))
            {
                isDeletedProperty.SetValue(entity, true);
                
                // Imposta anche UpdatedAt se disponibile
                var updatedAtProperty = typeof(T).GetProperty("UpdatedAt");
                if (updatedAtProperty != null && updatedAtProperty.PropertyType == typeof(DateTime?))
                {
                    updatedAtProperty.SetValue(entity, DateTime.UtcNow);
                }
                
                _logger.LogDebug("Successfully soft deleted entity {EntityType} with ID {Id}", typeof(T).Name, id);
                return Result.Success();
            }
            else
            {
                _logger.LogWarning("Entity {EntityType} does not support soft delete (no IsDeleted property)", typeof(T).Name);
                return Result.Failure($"Entity {typeof(T).Name} does not support soft delete");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting entity {EntityType} with ID {Id}", typeof(T).Name, id);
            return Result.Failure(ex);
        }
    }
}