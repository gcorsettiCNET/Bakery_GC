using Bakery.Application.Commands.Products;
using Bakery.Application.DTOs;
using Bakery.Core.Common;
using Bakery.Core.Entities.Products;
using Bakery.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bakery.Application.Handlers.Products;

/// <summary>
/// Handler per il command CreateProductCommand
/// Implementa la logica di creazione di nuovi prodotti con business rules
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling CreateProductCommand for Product: {ProductName}", request.Name);

            // Validazione business rules
            var validationResult = ValidateCommand(request);
            if (validationResult.IsFailure)
            {
                _logger.LogWarning("Validation failed for CreateProductCommand: {Error}", validationResult.Error);
                return Result<ProductDto>.Failure(validationResult.Error);
            }

            // Inizia transazione per operazioni multiple
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Crea l'entity appropriata basata sulla categoria
                var product = CreateProductFromCommand(request);
                
                // Aggiungi al repository
                var productRepository = _unitOfWork.Repository<Product, Guid>();
                var addResult = await productRepository.AddAsync(product);
                
                if (addResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError("Failed to add product: {Error}", addResult.Error);
                    return Result<ProductDto>.Failure(addResult.Error);
                }

                // Salva le modifiche
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Converti in DTO per la response
                var productDto = MapToProductDto(addResult.Value);

                _logger.LogInformation("Successfully created product: {ProductId} - {ProductName}", 
                    addResult.Value.Id, addResult.Value.Name);

                return Result<ProductDto>.Success(productDto);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling CreateProductCommand for Product: {ProductName}", request.Name);
            return Result<ProductDto>.Failure($"An error occurred while creating product: {ex.Message}");
        }
    }

    /// <summary>
    /// Valida le business rules per la creazione del prodotto
    /// </summary>
    private Result<bool> ValidateCommand(CreateProductCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<bool>.Failure("Product name is required");

        if (command.Price <= 0)
            return Result<bool>.Failure("Product price must be greater than 0");

        if (string.IsNullOrWhiteSpace(command.Category))
            return Result<bool>.Failure("Product category is required");

        // Validazioni specifiche per tipo di prodotto
        return command.Category.ToLowerInvariant() switch
        {
            "pizza" => ValidatePizzaSpecificFields(command),
            "bread" => ValidateBreadSpecificFields(command),
            "cake" => ValidateCakeSpecificFields(command),
            "pastrie" => ValidatePastrieSpecificFields(command),
            _ => Result<bool>.Success(true)
        };
    }

    private Result<bool> ValidatePizzaSpecificFields(CreateProductCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Ingredients))
            return Result<bool>.Failure("Pizza ingredients are required");
        
        return Result<bool>.Success(true);
    }

    private Result<bool> ValidateBreadSpecificFields(CreateProductCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.BreadType))
            return Result<bool>.Failure("Bread type is required");
        
        if (command.ShelfLifeDays.HasValue && command.ShelfLifeDays.Value <= 0)
            return Result<bool>.Failure("Shelf life days must be greater than 0");
        
        return Result<bool>.Success(true);
    }

    private Result<bool> ValidateCakeSpecificFields(CreateProductCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Flavor))
            return Result<bool>.Failure("Cake flavor is required");
        
        if (command.ServingSize.HasValue && command.ServingSize.Value <= 0)
            return Result<bool>.Failure("Serving size must be greater than 0");
        
        return Result<bool>.Success(true);
    }

    private Result<bool> ValidatePastrieSpecificFields(CreateProductCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.PastrieType))
            return Result<bool>.Failure("Pastrie type is required");
        
        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Crea l'entity Product appropriata basata sulla categoria
    /// </summary>
    private Product CreateProductFromCommand(CreateProductCommand command)
    {
        return command.Category.ToLowerInvariant() switch
        {
            "pizza" => new Pizza
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                IsAvailable = command.IsAvailable,
                Ingredients = command.Ingredients ?? string.Empty,
                Size = command.Size ?? string.Empty,
                IsSpicy = command.IsSpicy ?? false
            },
            "bread" => new Bread
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                IsAvailable = command.IsAvailable,
                BreadType = command.BreadType ?? string.Empty,
                IsGlutenFree = command.IsGlutenFree ?? false,
                ShelfLifeDays = command.ShelfLifeDays ?? 3
            },
            "cake" => new Cake
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                IsAvailable = command.IsAvailable,
                Flavor = command.Flavor ?? string.Empty,
                Occasion = command.Occasion ?? string.Empty,
                ServingSize = command.ServingSize ?? 1
            },
            "pastrie" => new Pastrie
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                IsAvailable = command.IsAvailable,
                PastrieType = command.PastrieType ?? string.Empty,
                Filling = command.Filling ?? string.Empty,
                IsVegan = command.IsVegan ?? false
            },
            _ => new Product
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                IsAvailable = command.IsAvailable
            }
        };
    }

    /// <summary>
    /// Mappa un'entity Product in ProductDto
    /// </summary>
    private static ProductDto MapToProductDto(Product product)
    {
        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description ?? string.Empty,
            Price = product.Price,
            IsAvailable = product.IsAvailable,
            Category = product.ProductType.ToString(),
            CreatedAt = product.CreatedAt
        };

        // Mappa le proprietà specifiche per tipo
        switch (product)
        {
            case Pizza pizza:
                dto.Ingredients = pizza.Ingredients;
                dto.Size = pizza.Size;
                dto.IsSpicy = pizza.IsSpicy;
                break;
                
            case Bread bread:
                dto.BreadType = bread.BreadType;
                dto.IsGlutenFree = bread.IsGlutenFree;
                dto.ShelfLifeDays = bread.ShelfLifeDays;
                break;
                
            case Cake cake:
                dto.Flavor = cake.Flavor;
                dto.Occasion = cake.Occasion;
                dto.ServingSize = cake.ServingSize;
                break;
                
            case Pastrie pastrie:
                dto.PastrieType = pastrie.PastrieType;
                dto.Filling = pastrie.Filling;
                dto.IsVegan = pastrie.IsVegan;
                break;
        }

        return dto;
    }
}