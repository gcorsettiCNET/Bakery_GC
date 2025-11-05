using Bakery.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bakery.Application.Extensions;

/// <summary>
/// Extension methods per configurare il layer Application
/// Configura MediatR, Behaviors e altre dipendenze dell'Application layer
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra tutti i servizi del layer Application
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection per method chaining</returns>
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // Registra MediatR con assembly scanning
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            
            // Registra i behaviors nell'ordine corretto
            // L'ordine è importante: prima logging request/response, poi performance
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestResponseLoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceLoggingBehavior<,>));
        });

        return services;
    }
}