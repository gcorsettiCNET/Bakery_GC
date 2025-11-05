using Bakery_GC.Models;
using Bakery_GC.Models.Local.ObjectToSell;

namespace Bakery_GC.Services
{
    public interface IProductService
    {
        // CRUD operations for Products
        Task<Result<IEnumerable<Product>>> GetPageProductsAsync(int pageNumber, int pageSize);
        Task<Result<Product>> GetProductByIdAsync(Guid id);
        Task<Result> AddProductAsync(Product product);
        Task<Result> UpdateProductAsync(Product product);
        Task<Result> DeleteProductAsync(Guid id);

    }
}
