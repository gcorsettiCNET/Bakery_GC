using Bakery.Core.Common;
using Bakery.Core.Entities.People;
using Bakery.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bakery.Infrastructure.Repositories;

/// <summary>
/// Implementazione del repository per Customers con logica business specifica
/// </summary>
public class CustomerRepository : Repository<Customer, Guid>, ICustomerRepository
{
    public CustomerRepository(DbContext context, ILogger<Repository<Customer, Guid>> logger) 
        : base(context, logger)
    {
    }

    public async Task<Result<IEnumerable<Customer>>> GetCustomersByMarketAsync(Guid marketId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting customers for market {MarketId}", marketId);

            var customers = await _dbSet
                .Where(c => c.MarketId == marketId && !c.IsDeleted)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {Count} customers for market {MarketId}", customers.Count, marketId);
            return Result<IEnumerable<Customer>>.Success(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customers for market {MarketId}", marketId);
            return Result<IEnumerable<Customer>>.Failure(ex);
        }
    }

    public async Task<Result<Customer>> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Result<Customer>.Failure("Email cannot be empty");
            }

            _logger.LogDebug("Getting customer by email {Email}", email);

            var customer = await _dbSet
                .FirstOrDefaultAsync(c => c.Email == email && !c.IsDeleted, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning("Customer with email {Email} not found", email);
                return Result<Customer>.Failure($"Customer with email {email} not found");
            }

            _logger.LogDebug("Found customer {CustomerId} with email {Email}", customer.Id, email);
            return Result<Customer>.Success(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer by email {Email}", email);
            return Result<Customer>.Failure(ex);
        }
    }

    public async Task<Result<IEnumerable<Customer>>> GetVipCustomersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting VIP customers");

            var customers = await _dbSet
                .Where(c => c.IsVip && !c.IsDeleted)
                .OrderByDescending(c => c.TotalSpent)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {Count} VIP customers", customers.Count);
            return Result<IEnumerable<Customer>>.Success(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting VIP customers");
            return Result<IEnumerable<Customer>>.Failure(ex);
        }
    }

    public async Task<Result<IEnumerable<Customer>>> GetRegularCustomersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting regular customers");

            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var customers = await _dbSet
                .Where(c => !c.IsDeleted && 
                           c.LastOrderDate.HasValue && 
                           c.LastOrderDate.Value >= thirtyDaysAgo &&
                           c.TotalSpent > 100)
                .OrderByDescending(c => c.LastOrderDate)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {Count} regular customers", customers.Count);
            return Result<IEnumerable<Customer>>.Success(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting regular customers");
            return Result<IEnumerable<Customer>>.Failure(ex);
        }
    }

    public async Task<Result<IEnumerable<Customer>>> GetTopSpendersAsync(int take = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting top {Take} spenders", take);

            var customers = await _dbSet
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.TotalSpent)
                .Take(take)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Found {Count} top spenders", customers.Count);
            return Result<IEnumerable<Customer>>.Success(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top spenders");
            return Result<IEnumerable<Customer>>.Failure(ex);
        }
    }
}