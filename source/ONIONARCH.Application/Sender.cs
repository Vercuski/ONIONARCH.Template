using Microsoft.Extensions.DependencyInjection;
using ONIONARCH.Application.Abstractions;

namespace ONIONARCH.Application;

/// <summary>
/// Reflection-based, in-process replacement for MediatR's Mediator class. Deliberately small: it
/// resolves the single closed <see cref="IRequestHandler{TRequest,TResponse}"/> for whatever
/// concrete request type was sent, then threads that call through every registered
/// <see cref="IPipelineBehavior{TRequest,TResponse}"/> in registration order (outermost first),
/// mirroring the behavior chain MediatR's AddOpenBehavior used to build.
/// </summary>
/// <param name="provider">The scoped service provider used to resolve handlers and behaviors.</param>
internal sealed class Sender(IServiceProvider provider) : ISender
{
    /// <inheritdoc />
    /// <remarks>
    /// The handler and behaviors are closed over the request's <em>runtime</em> type and invoked via
    /// <see langword="dynamic"/>, because <typeparamref name="TResponse"/> is the only generic
    /// argument known at compile time.
    /// </remarks>
    public Task<TResponse> Send<TResponse>(IAppRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        dynamic handler = provider.GetRequiredService(handlerType);

        Func<Task<TResponse>> pipeline = () => handler.Handle((dynamic)request, cancellationToken);

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviors = (IEnumerable<object>)provider.GetServices(behaviorType)!;

        // Reverse so the first-registered behavior ends up as the outermost wrapper — same
        // ordering semantics as MediatR's AddOpenBehavior.
        foreach (var behavior in behaviors.Reverse())
        {
            var next = pipeline;
            dynamic currentBehavior = behavior;
            pipeline = () => currentBehavior.Handle((dynamic)request, next, cancellationToken);
        }

        return pipeline();
    }
}
