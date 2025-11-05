using Bakery_GC.Models;
using Bakery_GC.Models.Local.HumanResources;
using Bakery_GC.Models.Local.ObjectToSell;
using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;


namespace Bakery_GC.Repositories.Interfaces
{
    public interface IProductRepository
    {
        // CRUD operations for Products
        Task<Result<IEnumerable<Product>>> GetPageProductsAsync(int pageNumber, int pageSize);
        Task<Result<Product>> GetProductByIdAsync(Guid id);
        Task<Result> AddProductAsync(Product product);
        Task<Result> UpdateProductAsync(Product product);
        Task<Result> DeleteProductAsync(Guid id);
        Task<Result<IEnumerable<Product>>> GetProductsByAvailabilityInMarketAsync(Market market);

        // Additional methods specific to product management
        Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(PizzaType type);
        Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(BreadType type);
        Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(PastryType type);
        Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(CakeType type);
        Task<Result<IEnumerable<Product>>> GetProductsByTypeAsync(ProductType type);
        Task<Result<IEnumerable<Product>>> GetProductsByFilterAsync(ProductFilter filter);
    }
}