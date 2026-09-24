using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Behaviors;
using System.Reflection;

namespace ONIONARCH.Application;

/// <summary>
/// Composition-root extensions that register the Application layer's services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the Application layer: the <see cref="ISender"/> dispatcher, every request handler
    /// in this assembly, and the pipeline behaviors.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    public static IHostApplicationBuilder AddApplicationRegistration(this IHostApplicationBuilder builder)
    {
        builder.AddMediatorRegistration();
        return builder;
    }

    /// <summary>
    /// Registers the in-process mediator: <see cref="Sender"/> (scoped), all request handlers
    /// (scoped), and <see cref="LoggingBehavior{TRequest,TResponse}"/> as an open-generic
    /// pipeline behavior (transient).
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    /// <remarks>
    /// Behaviors run in registration order (first registered is outermost); add further
    /// <c>IPipelineBehavior&lt;,&gt;</c> registrations here in the order they should wrap.
    /// </remarks>
    private static IHostApplicationBuilder AddMediatorRegistration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISender, Sender>();
        // Scanning only this assembly (rather than every loaded assembly, as MediatR's
        // RegisterServicesFromAssemblies previously did) is deliberate: every handler in this
        // template lives in ONIONARCH.Application. If a future slice adds handlers in another
        // assembly, pass its Assembly alongside GetExecutingAssembly() here.
        builder.Services.AddRequestHandlers(Assembly.GetExecutingAssembly());
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return builder;
    }

    /// <summary>
    /// Reflection-scans the given assembly for every non-abstract, non-interface type that
    /// implements a closed <see cref="IRequestHandler{TRequest,TResponse}"/> and registers it
    /// against that interface. A handler declared as IQueryHandler&lt;,&gt;/ICommandHandler&lt;,&gt; is still
    /// found here because Type.GetInterfaces() returns the full transitive closure, including the
    /// IRequestHandler&lt;,&gt; those interfaces inherit from — no separate registration path is needed
    /// per marker interface. Scoped to mirror the lifetime EF Core's DbContext-backed handlers need.
    /// </summary>
    /// <param name="services">The service collection to register handlers with.</param>
    /// <param name="assembly">The assembly to scan for handler implementations.</param>
    /// <returns>The same <paramref name="services"/>, for chaining.</returns>
    private static IServiceCollection AddRequestHandlers(this IServiceCollection services, Assembly assembly)
    {
        var openHandlerType = typeof(IRequestHandler<,>);

        foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface))
        {
            foreach (var candidateInterface in type.GetInterfaces())
            {
                if (candidateInterface.IsGenericType
                    && candidateInterface.GetGenericTypeDefinition() == openHandlerType)
                {
                    services.AddScoped(candidateInterface, type);
                }
            }
        }

        return services;
    }
}
