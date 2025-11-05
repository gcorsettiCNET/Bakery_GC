using Bakery.Application.DTOs;
using Bakery.Core.Common;
using MediatR;

namespace Bakery.Application.Queries.Products;

/// <summary>
/// Query per ottenere un prodotto specifico per ID
/// </summary>
public class GetProductByIdQuery : IRequest<Result<ProductDto>>
{
    public Guid ProductId { get; set; }
    
    public GetProductByIdQuery(Guid productId)
    {
        ProductId = productId;
    }
}