using Bakery_GC.Models.Local.ObjectToSell;
using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;
using HL7Analyzer.Models;

namespace Bakery_GC.Repositories
{
    public interface IProductRepository
    {
        // Asynchronous methods for CRUD operations
        // CRUD operations for Products
        Task<Result<IEnumerable<Product>>> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<Result<Product>> GetProductByIdAsync(int id);
        Task<Result> AddProductAsync(Product product);
        Task<Result> UpdateProductAsync(Product product);
        Task<Result> DeleteProductAsync(int id);
        // Additional methods specific to product management
        Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(PizzaType type);
        Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(BreadType type);
        Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(PastryType type);
        Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(CakeType type);
        Task<Result<IEnumerable<Product>>> GetProductsCategoryAsync();
        Task<Result<IEnumerable<Product>>> SearchProductsAsync(string searchTerm);
        Task<Result<IEnumerable<Product>>> GetFeaturedProductsAsync(int count);
        Task<Result<IEnumerable<Product>>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<Result<IEnumerable<Product>>> GetProductsByMarketAsync(int marketId);
        Task<Result<IEnumerable<Product>>> GetProductsByAvailabilityAsync(bool isAvailable);
    }
}