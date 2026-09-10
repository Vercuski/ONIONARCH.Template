using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Behaviors;
using System.Reflection;

namespace ONIONARCH.Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplicationRegistration(this IHostApplicationBuilder builder)
    {
        builder.AddMediatorRegistration();
        return builder;
    }

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
