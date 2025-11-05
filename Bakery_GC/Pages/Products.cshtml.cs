using Bakery_GC.Models;
using Bakery_GC.Models.Local.ObjectToSell;
using Bakery_GC.Models.Local.ObjectToSell.TypeEnum;
using Bakery_GC.Models.Local.Orders;
using Bakery_GC.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bakery_GC.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly IProductRepository _productRepository;

        public ProductsModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Binding del filtro (usa la notazione Filter.X nei name del form)
        [BindProperty(SupportsGet = true)]
        public ProductFilter Filter { get; set; } = new();

        // Paginazione basilare
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 12;

        public IReadOnlyList<Product> Products { get; private set; } = Array.Empty<Product>();
        public bool HasActiveFilter =>
            Filter.ProductType.HasValue ||
            Filter.BreadType.HasValue ||
            Filter.CakeType.HasValue ||
            Filter.PastryType.HasValue ||
            Filter.PizzaType.HasValue ||
            Filter.MarketId.HasValue ||
            !string.IsNullOrWhiteSpace(Filter.NameMarket) ||
            Filter.MinPrice.HasValue ||
            Filter.MaxPrice.HasValue ||
            !string.IsNullOrWhiteSpace(Filter.SearchTerm);

        public async Task OnGetAsync()
        {
            // Se ci sono filtri attivi uso il repository filtrato
            if (HasActiveFilter)
            {
                var filtered = await _productRepository.GetProductsByFilterAsync(Filter);
                if (filtered.IsSuccess)
                    Products = filtered.Value.ToList();
                else
                    Products = [];
            }
            else
            {
                // Lista paginata base
                var page = await _productRepository.GetPageProductsAsync(PageNumber, PageSize);
                if (page.IsSuccess)
                    Products = page.Value.ToList();
            }
        }
    }
}
