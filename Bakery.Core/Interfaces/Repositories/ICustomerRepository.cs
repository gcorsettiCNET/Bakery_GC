using Bakery.Core.Common;
using Bakery.Core.Entities.People;

namespace Bakery.Core.Interfaces.Repositories;

/// <summary>
/// Repository specifico per Customers con metodi business-oriented
/// </summary>
public interface ICustomerRepository : IRepository<Customer, Guid>
{
    /// <summary>
    /// Ottiene clienti per market
    /// </summary>
    Task<Result<IEnumerable<Customer>>> GetCustomersByMarketAsync(Guid marketId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Trova cliente per email
    /// </summary>
    Task<Result<Customer>> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ottiene clienti VIP
    /// </summary>
    Task<Result<IEnumerable<Customer>>> GetVipCustomersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ottiene clienti abituali (con ordini recenti)
    /// </summary>
    Task<Result<IEnumerable<Customer>>> GetRegularCustomersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ottiene top spenders
    /// </summary>
    Task<Result<IEnumerable<Customer>>> GetTopSpendersAsync(int take = 10, CancellationToken cancellationToken = default);
}