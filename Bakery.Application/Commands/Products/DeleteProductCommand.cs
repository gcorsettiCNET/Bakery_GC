using Bakery.Core.Common;
using MediatR;

namespace Bakery.Application.Commands.Products;

/// <summary>
/// Command per eliminare un prodotto
/// </summary>
public class DeleteProductCommand : IRequest<Result<bool>>
{
    public Guid ProductId { get; set; }
    
    public DeleteProductCommand(Guid productId)
    {
        ProductId = productId;
    }
}