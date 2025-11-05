using Bakery.Core.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bakery.Application.Behaviors;

/// <summary>
/// Behavior per il logging delle richieste e risposte CQRS
/// Fornisce trasparenza completa su tutte le operazioni
/// </summary>
/// <typeparam name="TRequest">Tipo della request</typeparam>
/// <typeparam name="TResponse">Tipo della response</typeparam>
public class RequestResponseLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> _logger;

    public RequestResponseLoggingBehavior(ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();

        // Log della richiesta
        _logger.LogInformation("Processing {RequestName} with Id {RequestId}: {Request}", 
            requestName, requestId, System.Text.Json.JsonSerializer.Serialize(request));

        try
        {
            var response = await next();

            // Log della risposta (solo per successi)
            if (IsResultType(response) && IsFailureResult(response))
            {
                _logger.LogWarning("Request {RequestName} with Id {RequestId} completed with failure", 
                    requestName, requestId);
            }
            else
            {
                _logger.LogInformation("Request {RequestName} with Id {RequestId} completed successfully", 
                    requestName, requestId);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request {RequestName} with Id {RequestId} failed with exception", 
                requestName, requestId);
            throw;
        }
    }

    /// <summary>
    /// Verifica se la response è di tipo Result<T>
    /// </summary>
    private static bool IsResultType(TResponse response)
    {
        if (response == null) return false;
        
        var responseType = response.GetType();
        return responseType.IsGenericType && 
               (responseType.GetGenericTypeDefinition() == typeof(Result<>) || 
                responseType.Name.StartsWith("Result"));
    }

    /// <summary>
    /// Verifica se un Result<T> rappresenta un fallimento
    /// </summary>
    private static bool IsFailureResult(TResponse response)
    {
        if (response == null) return true;

        // Usa reflection per accedere alla proprietà IsFailure se esiste
        var isFailureProperty = response.GetType().GetProperty("IsFailure");
        if (isFailureProperty != null)
        {
            return (bool)(isFailureProperty.GetValue(response) ?? true);
        }

        return false;
    }
}