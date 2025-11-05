using Bakery_GC.Models;
using Bakery_GC.Models.Local.ObjectToSell;
using Bakery_GC.Repositories.Interfaces;

namespace Bakery_GC.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo) => _repo = repo;

        public Task<Result> AddProductAsync(Product product) =>
            _repo.AddProductAsync(product);

        public Task<Result> UpdateProductAsync(Product product) =>
            _repo.UpdateProductAsync(product);

        public Task<Result> DeleteProductAsync(Guid id) =>
            _repo.DeleteProductAsync(id);

        public Task<Result<Product>> GetProductByIdAsync(Guid id) =>
            _repo.GetProductByIdAsync(id);

        public Task<Result<IEnumerable<Product>>> GetPageProductsAsync(int pageNumber, int pageSize) =>
            _repo.GetPageProductsAsync(pageNumber, pageSize);
    }
}
