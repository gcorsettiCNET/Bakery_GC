using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Bakery.Application.Behaviors;

/// <summary>
/// Behavior per il logging delle performance delle operazioni CQRS
/// Implementa il pattern Pipeline Behavior di MediatR per cross-cutting concerns
/// </summary>
/// <typeparam name="TRequest">Tipo della request (Command o Query)</typeparam>
/// <typeparam name="TResponse">Tipo della response</typeparam>
public class PerformanceLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly ILogger<PerformanceLoggingBehavior<TRequest, TResponse>> _logger;
    private const int SlowRequestThresholdMs = 500;

    public PerformanceLoggingBehavior(ILogger<PerformanceLoggingBehavior<TRequest, TResponse>> logger)
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

        _logger.LogInformation("Starting request {RequestName} with Id {RequestId}", 
            requestName, requestId);

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (elapsedMs > SlowRequestThresholdMs)
            {
                _logger.LogWarning("Slow request detected - {RequestName} with Id {RequestId} took {ElapsedMs}ms", 
                    requestName, requestId, elapsedMs);
            }
            else
            {
                _logger.LogInformation("Completed request {RequestName} with Id {RequestId} in {ElapsedMs}ms", 
                    requestName, requestId, elapsedMs);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            
            _logger.LogError(ex, "Request {RequestName} with Id {RequestId} failed after {ElapsedMs}ms", 
                requestName, requestId, elapsedMs);
            
            throw;
        }
    }
}