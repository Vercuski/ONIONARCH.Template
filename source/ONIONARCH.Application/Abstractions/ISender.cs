namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Single dispatch chokepoint for the CQRS pipeline. Presentation depends only on this interface
/// and on request/response contract types — never on a concrete handler — which is the same
/// decoupling MediatR's IMediator previously provided.
/// </summary>
public interface ISender
{
    /// <summary>
    /// Dispatches <paramref name="request"/> through every registered
    /// <see cref="IPipelineBehavior{TRequest,TResponse}"/> to its single
    /// <see cref="IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <typeparam name="TResponse">The response type declared by the request.</typeparam>
    /// <param name="request">The query or command to dispatch.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response produced by the request's handler.</returns>
    /// <exception cref="InvalidOperationException">No handler is registered for the request's runtime type.</exception>
    Task<TResponse> Send<TResponse>(IAppRequest<TResponse> request, CancellationToken cancellationToken = default);
}
