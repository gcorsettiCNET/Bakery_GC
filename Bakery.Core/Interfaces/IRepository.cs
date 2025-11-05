using Bakery.Core.Common;
using System.Linq.Expressions;

namespace Bakery.Core.Interfaces;

/// <summary>
/// Generic Repository Interface per operazioni CRUD base
/// T = Entity Type, TKey = Primary Key Type
/// </summary>
public interface IRepository<T, TKey> where T : class
{
    // ======= QUERY METHODS =======
    
    /// <summary>
    /// Ottiene entità per ID
    /// </summary>
    Task<Result<T>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ottiene tutte le entità
    /// </summary>
    Task<Result<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Trova entità con filtro
    /// </summary>
    Task<Result<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ottiene prima entità che soddisfa il filtro
    /// </summary>
    Task<Result<T>> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Verifica se esiste entità con filtro
    /// </summary>
    Task<Result<bool>> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Conta entità con filtro
    /// </summary>
    Task<Result<int>> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    
    // ======= COMMAND METHODS =======
    
    /// <summary>
    /// Aggiunge una nuova entità
    /// </summary>
    Task<Result<T>> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Aggiunge multiple entità
    /// </summary>
    Task<Result<IEnumerable<T>>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Aggiorna entità esistente
    /// </summary>
    Result<T> Update(T entity);
    
    /// <summary>
    /// Aggiorna multiple entità
    /// </summary>
    Result<IEnumerable<T>> UpdateRange(IEnumerable<T> entities);
    
    /// <summary>
    /// Rimuove entità per ID
    /// </summary>
    Task<Result> RemoveAsync(TKey id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rimuove entità
    /// </summary>
    Result Remove(T entity);
    
    /// <summary>
    /// Rimuove multiple entità
    /// </summary>
    Result RemoveRange(IEnumerable<T> entities);
    
    // ======= SOFT DELETE =======
    
    /// <summary>
    /// Soft delete per ID (imposta IsDeleted = true)
    /// </summary>
    Task<Result> SoftDeleteAsync(TKey id, CancellationToken cancellationToken = default);
}