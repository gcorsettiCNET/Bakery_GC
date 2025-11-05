using Bakery_GC.Models;
using Bakery_GC.Models.Local;
using Bakery_GC.Models.Local.HumanResources;
using Bakery_GC.Models.Local.ObjectToSell;
using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;
using Bakery_GC.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bakery_GC.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly Models.ApplicationDBContext_Local _dbContext;

        public ProductRepository(Models.ApplicationDBContext_Local dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result> AddProductAsync(Product product)
        {
            if (product is null)
                return Result.Failure(Error.InvalidInput);

            try
            {
                await _dbContext.Products.AddAsync(product);
                var written = await _dbContext.SaveChangesAsync();
                if (written <= 0)
                    return Result.Failure(Error.UnexpectedError);
                return Result.Success();
            }
            catch (DbUpdateException)
            {
                return Result.Failure(Error.DuplicatedEntry);
            }
            catch
            {
                return Result.Failure(Error.UnexpectedError);
            }
        }

        public async Task<Result> UpdateProductAsync(Product product)
        {
            if (product is null || product.Id == Guid.Empty)
                return Result.Failure(Error.InvalidInput);

            var exists = await _dbContext.Products
                .AsNoTracking()
                .AnyAsync(p => p.Id == product.Id);

            if (!exists)
                return Result.Failure(Error.NotFound);

            _dbContext.Products.Update(product);

            try
            {
                var written = await _dbContext.SaveChangesAsync();
                if (written <= 0)
                    return Result.Failure(Error.UnexpectedError);
                return Result.Success();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Failure(Error.UnexpectedError);
            }
        }

        public async Task<Result> DeleteProductAsync(Guid id)
        {
            if (id == Guid.Empty)
                return Result.Failure(Error.InvalidInput);

            var affected = await _dbContext
                .Products
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();

            if (affected == 0)
                return Result.Failure(Error.NotFound);

            return Result.Success();
        }

        public async Task<Result<Product>> GetProductByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return Result<Product>.Failure(Error.InvalidInput);

            if (await _dbContext.Products.FindAsync(id) is Product product)
                return Result<Product>.Success(product);

            return Result<Product>.Failure(Error.NotFound);
        }

        public async Task<Result<IEnumerable<Product>>> GetPageProductsAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Result<IEnumerable<Product>>.Failure(Error.InvalidPaging);

            var products = await _dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<IEnumerable<Product>>.Success(products);
        }

        public async Task<Result<IEnumerable<Product>>> GetProductsByAvailabilityInMarketAsync(Market market)
        {
            if (market is null || market.Id == Guid.Empty)
                return Result<IEnumerable<Product>>.Failure(Error.InvalidInput);

            var marketExists = await _dbContext.Markets
                .AsNoTracking()
                .AnyAsync(m => m.Id == market.Id);

            if (!marketExists)
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);

            var products = await _dbContext.Products
                .Where(p => p.Market.Id == market.Id)
                .AsNoTracking()
                .ToListAsync();

            if (products.Count == 0)
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);

            return Result<IEnumerable<Product>>.Success(products);
        }

        private IQueryable<TDerived> QueryDerived<TDerived>(ProductType typeDiscriminator)
            where TDerived : Product
        {
            return _dbContext.Products
                .Where(p => p.ProductType == typeDiscriminator)
                .OfType<TDerived>()
                .AsNoTracking();
        }

        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(BreadType type)
        {
            var list = await QueryDerived<Bread>(ProductType.Bread)
                .Where(b => b.BreadType == type)
                .ToListAsync();

            if (list.Count == 0)
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);

            return Result<IEnumerable<Product>>.Success(list);
        }

        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(CakeType type)
        {
            var list = await QueryDerived<Cake>(ProductType.Cake)
                .Where(c => c.CakeType == type)
                .ToListAsync();

            if (list.Count == 0)
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);

            return Result<IEnumerable<Product>>.Success(list);
        }

        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(PastryType type)
        {
            var list = await QueryDerived<Pastrie>(ProductType.Pastrie)
                .Where(p => p.PastryType == type)
                .ToListAsync();

            if (list.Count == 0)
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);

            return Result<IEnumerable<Product>>.Success(list);
        }

        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(PizzaType type)
        {
            var list = await QueryDerived<Pizza>(ProductType.Pizza)
                .Where(p => p.PizzaType == type)
                .ToListAsync();

            if (list.Count == 0)
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);

            return Result<IEnumerable<Product>>.Success(list);
        }

        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(ProductType type)
        {
            var list = await _dbContext.Products.Where(p => p.ProductType == type)
                .AsNoTracking()
                .ToListAsync();

            if (list.Count == 0)
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);

            return Result<IEnumerable<Product>>.Success(list);
        }

        // -------- Refactored Flexible Filter ----------
        public async Task<Result<IEnumerable<Product>>> GetProductsByFilterAsync(ProductFilter filter)
        {
            if (filter is null)
                return Result<IEnumerable<Product>>.Failure(Error.InvalidInput);

            IQueryable<Product> query = _dbContext.Products.AsNoTracking();

            // 1. Discriminator (if explicitly provided)
            if (filter.ProductType.HasValue)
                query = query.Where(p => p.ProductType == filter.ProductType.Value);

            // 2. Subtype-specific filters (applied even if ProductType not explicitly set)
            if (filter.PizzaType.HasValue)
            {
                query = query.Where(p => p.ProductType == ProductType.Pizza)
                             .OfType<Pizza>()
                             .Where(pz => pz.PizzaType == filter.PizzaType.Value);
            }
            else if (filter.BreadType.HasValue)
            {
                query = query.Where(p => p.ProductType == ProductType.Bread)
                             .OfType<Bread>()
                             .Where(b => b.BreadType == filter.BreadType.Value);
            }
            else if (filter.CakeType.HasValue)
            {
                query = query.Where(p => p.ProductType == ProductType.Cake)
                             .OfType<Cake>()
                             .Where(c => c.CakeType == filter.CakeType.Value);
            }
            else if (filter.PastryType.HasValue)
            {
                query = query.Where(p => p.ProductType == ProductType.Pastrie)
                             .OfType<Pastrie>()
                             .Where(ps => ps.PastryType == filter.PastryType.Value);
            }

            // 3. Market filtering
            if (filter.MarketId.HasValue)
                query = query.Where(p => p.Market.Id == filter.MarketId.Value);

            if (!string.IsNullOrWhiteSpace(filter.NameMarket))
                query = query.Where(p => p.Market.Name == filter.NameMarket);

            // 4. Availability (if you later add such a flag to Product or a join table)
            if (filter.IsAvailable.HasValue)
            {
                // Placeholder: adapt when availability is modeled
                query = query.Where(p => p.IsAvailable == filter.IsAvailable.Value);
            }

            // 5. Text search (Name + Description)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = $"%{filter.SearchTerm.Trim()}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, term) ||
                    (p.Description != null && EF.Functions.Like(p.Description, term))
                );
            }

            // 6. (Optional) ordering default
            query = query.OrderBy(p => p.Name).ThenBy(p => p.Id);

            var products = await query.ToListAsync();

            if (products.Count == 0)
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);

            return Result<IEnumerable<Product>>.Success(products);
        }
    }
}

