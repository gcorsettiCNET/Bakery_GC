using Bakery.Core.Common;

namespace Bakery.Core.Interfaces;

/// <summary>
/// Unit of Work pattern per gestione transazioni
/// Coordina il lavoro di multiple repository e garantisce consistenza
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // ======= REPOSITORY ACCESS =======
    
    /// <summary>
    /// Ottiene repository generico per tipo T
    /// </summary>
    IRepository<T, TKey> Repository<T, TKey>() where T : class;
    
    // ======= TRANSACTION MANAGEMENT =======
    
    /// <summary>
    /// Inizia una nuova transazione
    /// </summary>
    Task<Result> BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commit della transazione corrente
    /// </summary>
    Task<Result> CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rollback della transazione corrente
    /// </summary>
    Task<Result> RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
    // ======= SAVE CHANGES =======
    
    /// <summary>
    /// Salva tutti i cambiamenti nel database
    /// </summary>
    Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Salva i cambiamenti con transazione automatica
    /// </summary>
    Task<Result<int>> SaveChangesWithTransactionAsync(CancellationToken cancellationToken = default);
    
    // ======= HEALTH CHECK =======
    
    /// <summary>
    /// Verifica se ci sono cambiamenti pending
    /// </summary>
    bool HasPendingChanges();
    
    /// <summary>
    /// Ottiene il numero di cambiamenti pending
    /// </summary>
    int GetPendingChangesCount();
}