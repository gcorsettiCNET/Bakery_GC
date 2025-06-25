using Bakery_GC.Models.Local;
using Bakery_GC.Models.Local.HumanResources;
using Bakery_GC.Models.Local.ObjectToSell;
using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;
using HL7Analyzer.Models;
using Microsoft.EntityFrameworkCore;

namespace Bakery_GC.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext_Local _dbContext;
        public ProductRepository( ApplicationDBContext_Local dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds a new product to the repository asyncronously.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        /// <exception cref="AlreadyExistingProductException"></exception>
        public async Task<Result> AddProductAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            if(await _dbContext.SaveChangesAsync() <= 0)
            {
                return Result.Failure(Error.DuplicatedEntry);
            }
            return Result.Success();
        }

        public async Task<Result> DeleteProductAsync(Guid id)
        {
            if (await _dbContext.Products.Where(p => p.Id == id).ExecuteDeleteAsync() <= 0)
            {
                return Result.Failure(Error.NotFound);
            }
            return Result.Success();
        }

        public async Task<Result<IEnumerable<Product>>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return Result<IEnumerable<Product>>.Failure(Error.InvalidPaging);
            }

            var products = await _dbContext.Products
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Result<IEnumerable<Product>>.Success(products);
        }

        public async Task<Result<Product>> GetProductByIdAsync(Guid id)
        {
            if( await _dbContext.Products.FindAsync(id) is Product product )
            {
                return Result<Product>.Success(product);
            }
            return Result<Product>.Failure(Error.NotFound);
        }

        public async Task<Result<IEnumerable<Product>>> GetProductsByAvailabilityInMarketAsync(Market market)
        {
            // Controllo che il negozio passato esista
            if (market == null || !await _dbContext.Markets.AnyAsync(m => m.Id == market.Id))
            {
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);
            }

            // Recupero i prodotti che sono disponibili nel mercato specificato
            var products = await _dbContext.Products
                .Where(x => x.Market.Id == market.Id)
                .ToListAsync();
            if ( !products.Any() )
            {
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);
            }
            return Result<IEnumerable<Product>>.Success(products);
        }
        // Overload per Bread
        public Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(BreadType type)
        {   
            // Un prodotto può essere di più tipi e in base al tipo questo ha delle enum, prima si capisce il tipo e poi si fa la select sulla enum.
        }
        // Overload per Cake
        public Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(CakeType type)
        {
            // Un prodotto può essere di più tipi e in base al tipo questo ha delle enum, prima si capisce il tipo e poi si fa la select sulla enum.
        }
        // Overload per Pastries
        public Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(PastryType type )
        {
            // Un prodotto può essere di più tipi e in base al tipo questo ha delle enum, prima si capisce il tipo e poi si fa la select sulla enum.
        }
        // Overload per Pizza
        public Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(PizzaType type )
        {
            // Un prodotto può essere di più tipi e in base al tipo questo ha delle enum, prima si capisce il tipo e poi si fa la select sulla enum.
        }
        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(ProductType type)
        {
            var products = await _dbContext.Products
                .Where(p => EF.Property<ProductType>(p, "ProductType") == type)
                .ToListAsync();

            return Result<IEnumerable<Product>>.Success(products);
        }

        public async Task<Result<IEnumerable<Product>>> GetProductsByFilterAsync(ProductFilter filter)
        {
            var query = _dbContext.Products.AsQueryable();

            if (filter.ProductType.HasValue)
                query = query.Where(p => EF.Property<ProductType>(p, "ProductType") == filter.ProductType.Value);

            if (filter.PizzaType.HasValue && filter.ProductType.Value == ProductType.Pizza)
                query = query.Where(p => p. == filter.PizzaType.Value);

            if (filter.BreadType.HasValue)
                query = query.Where(p => p is Bread bread && bread.BreadType == filter.BreadType.Value);

            if (filter.CakeType.HasValue)
                query = query.Where(p => p is Cake cake && cake.CakeType == filter.CakeType.Value);

            if (filter.PastryType.HasValue)
                query = query.Where(p => p is Pastrie pastrie && pastrie.PastryType == filter.PastryType.Value);

            if (filter.MarketId.HasValue)
                query = query.Where(p => p.Market.Id == filter.MarketId.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => EF.Property<decimal>(p, "Price") >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => EF.Property<decimal>(p, "Price") <= filter.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                query = query.Where(p => EF.Property<string>(p, "Name").Contains(filter.SearchTerm));

            // Aggiungi altri filtri se necessario

            var products = await query.ToListAsync();
            return Result<IEnumerable<Product>>.Success(products);
        }


        public Task<Result<IEnumerable<Product>>> GetProductsByMarketAsync(int marketId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<Product>>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<Product>>> SearchProductsAsync(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
