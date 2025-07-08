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
        // CRUD operations for Products ( Non create )

        /// <summary>
        /// Adds a new product to the repository asynchronously.
        /// </summary>
        /// <param name="product">The product to add. <see cref="Product"/> </param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        /// <exception> <see cref="Error.DuplicatedEntry"/></exception>
        public async Task<Result> AddProductAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            if(await _dbContext.SaveChangesAsync() <= 0)
            {
                return Result.Failure(Error.DuplicatedEntry);
            }
            return Result.Success();
        }
        /// <summary>
        /// Updates an existing product in the repository asyncronously.
        /// </summary>
        /// <param name="product"></param>
        /// <returns> <see cref="Error.InvalidInput"/> If the product that has been passed to the method is null or the Id is an Empty Guid 
        /// <see cref="Error.NotFound"/> If the product that has been passed cannot be found in the database
        /// <see cref="Error.UnexpectedError"/> If the product exist and something went wrong in the update in the database
        /// <see cref="Result"/> If the product has been updated successfully </returns>
        public async Task<Result> UpdateProductAsync(Product product)
        {
            if (product == null || product.Id == Guid.Empty)
            {
                return Result.Failure(Error.InvalidInput);
            }

            var productToUpdate = await _dbContext.Products.Where(x => x.Id == product.Id).AsNoTracking().FirstAsync();
            if (productToUpdate == null)
            {
                return Result.Failure(Error.NotFound);
            }
            else
            {
                _dbContext.Products.Update(product);
                if (await _dbContext.SaveChangesAsync() <= 0)
                {
                    return Result.Failure(Error.UnexpectedError);
                }
                return Result.Success();
            }
        }
        /// <summary>
        /// Deletes a product from the repository asyncronously by its ID.
        /// </summary>
        /// <param name="id">The Guid of the Product we want to delete</param>
        /// <returns>A <see cref="Result"/> containing the Result of the operation </returns>
        /// <exception cref="Error.NotFound">Thrown if the product does not exist in the database </exception>
        public async Task<Result> DeleteProductAsync(Guid id)
        {
            if (await _dbContext.Products.Where(p => p.Id == id).ExecuteDeleteAsync() <= 0)
            {
                return Result.Failure(Error.NotFound);
            }
            return Result.Success();
        }
        /// <summary>
        /// Retrieves a product by its ID from the repository asynchronously.
        /// </summary>
        /// <param name="id">The Guid of the product.</param>
        /// <returns>A <see cref="Result{Product}"/> containing the product if found, otherwise <see cref="Error.NotFound"/>.</returns>
        public async Task<Result<Product>> GetProductByIdAsync(Guid id)
        {
            if( await _dbContext.Products.FindAsync(id) is Product product )
            {
                return Result<Product>.Success(product);
            }
            return Result<Product>.Failure(Error.NotFound);
        }
        /// <summary>
        /// Retrieves all products with optional pagination.
        /// </summary>
        /// <param name="pageNumber">The page number (1-based).</param>
        /// <param name="pageSize">The number of products per page.</param>
        /// <returns>A <see cref="Result{IEnumerable{Product}}"/> containing the paged products.</returns>
        /// <exception cref="Error.InvalidPaging">Thrown if pageNumber or pageSize is less than 1.</exception>
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

        // Additional methods specific to product management

        /// <summary>
        /// Retrieves all products available in a specific market.
        /// </summary>
        /// <param name="market">The market to filter by.</param>
        /// <returns>A <see cref="Result{IEnumerable{Product}}"/> containing the products available in the market.</returns>
        /// <exception cref="Error.NotFound">Returned if the market or products are not found.</exception>
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
        /// <summary>
        /// Retrieves products filtered by the specified <see cref="BreadType"/>.
        /// </summary>
        /// <param name="type">The bread type to filter by.</param>
        /// <returns>A <see cref="Result{IEnumerable{Product}}"/> containing the filtered products.</returns>
        /// <exception cref="Error.NotFound">Returned if no products match the filter.</exception>
        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(BreadType type)
        {
            var products = await _dbContext.Products
                .Where(p => EF.Property<ProductType>(p, "ProductType") == ProductType.Bread)
                .OfType<Bread>() // Filtra solo i prodotti di tipo Bread
                .Where(b => b.BreadType == type) // Filtra ulteriormente per BreadType
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);
            }
            return Result<IEnumerable<Product>>.Success(products);
        }
        /// <summary>
        /// Retrieves products filtered by the specified <see cref="CakeType"/>.
        /// </summary>
        /// <param name="type">The cake type to filter by.</param>
        /// <returns>A <see cref="Result{IEnumerable{Product}}"/> containing the filtered products.</returns>
        /// <exception cref="Error.NotFound">Returned if no products match the filter.</exception>
        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(CakeType type)
        {
            var products = await _dbContext.Products
                .Where(p => EF.Property<ProductType>(p, "ProductType") == ProductType.Cake)
                .OfType<Cake>() // Filtra solo i prodotti di tipo Bread
                .Where(b => b.CakeType == type) // Filtra ulteriormente per BreadType
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);
            }
            return Result<IEnumerable<Product>>.Success(products);
        }
        /// <summary>
        /// Retrieves products filtered by the specified <see cref="PastryType"/>.
        /// </summary>
        /// <param name="type">The pastry type to filter by.</param>
        /// <returns>A <see cref="Result{IEnumerable{Product}}"/> containing the filtered products.</returns>
        /// <exception cref="Error.NotFound">Returned if no products match the filter.</exception>
        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(PastryType type )
        {
            var products = await _dbContext.Products
                .Where(p => EF.Property<ProductType>(p, "ProductType") == ProductType.Pastrie)
                .OfType<Pastrie>() // Filtra solo i prodotti di tipo Bread
                .Where(b => b.PastryType == type) // Filtra ulteriormente per BreadType
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);
            }
            return Result<IEnumerable<Product>>.Success(products);
        }
        /// <summary>
        /// Retrieves products filtered by the specified <see cref="PizzaType"/>.
        /// </summary>
        /// <param name="type">The pizza type to filter by.</param>
        /// <returns>A <see cref="Result{IEnumerable{Product}}"/> containing the filtered products.</returns>
        /// <exception cref="Error.NotFound">Returned if no products match the filter.</exception>
        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(PizzaType type )
        {
            var products = await _dbContext.Products
                .Where(p => EF.Property<ProductType>(p, "ProductType") == ProductType.Pizza)
                .OfType<Pizza>() // Filtra solo i prodotti di tipo Bread
                .Where(b => b.PizzaType == type) // Filtra ulteriormente per BreadType
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return Result<IEnumerable<Product>>.Failure(Error.NotFound);
            }
            return Result<IEnumerable<Product>>.Success(products);
        }
        /// <summary>
        /// Retrieves products filtered by the specified <see cref="ProductType"/>.
        /// </summary>
        /// <param name="type">The product type to filter by.</param>
        /// <returns>A <see cref="Result{IEnumerable{Product}}"/> containing the filtered products.</returns>
        public async Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(ProductType type)
        {
            var products = await _dbContext.Products
                .Where(p => EF.Property<ProductType>(p, "ProductType") == type)
                .ToListAsync();

            return Result<IEnumerable<Product>>.Success(products);
        }
        /// <summary>
        /// Retrieves products matching the specified filter criteria.
        /// </summary>
        /// <param name="filter">The filter criteria to apply.</param>
        /// <returns>A <see cref="Result{IEnumerable{Product}}"/> containing the filtered products.</returns>
        public async Task<Result<IEnumerable<Product>>> GetProductsByFilterAsync(ProductFilter filter)
        {
            var query = _dbContext.Products.AsQueryable();

            if (filter.ProductType.HasValue)
                query = query.Where(p => EF.Property<ProductType>(p, "ProductType") == filter.ProductType.Value);

            if (filter.PizzaType.HasValue && filter.ProductType.HasValue && filter.ProductType.Value == ProductType.Pizza )
                query = query.OfType<Pizza>()
                    .Where(p => p.PizzaType == filter.PizzaType.Value);

            if (filter.BreadType.HasValue && filter.ProductType.HasValue && filter.ProductType.Value == ProductType.Bread )
                query = query.OfType<Bread>()
                    .Where(p => p.BreadType == filter.BreadType.Value);

            if (filter.CakeType.HasValue && filter.ProductType.HasValue && filter.ProductType.Value == ProductType.Cake )
                query = query.OfType<Cake>()
                    .Where(p => p.CakeType == filter.CakeType.Value);

            if (filter.PastryType.HasValue && filter.ProductType.HasValue && filter.ProductType.Value == ProductType.Pastrie )
                query = query.OfType<Pastrie>()
                    .Where(p => p.PastryType == filter.PastryType.Value);

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
    }
}

