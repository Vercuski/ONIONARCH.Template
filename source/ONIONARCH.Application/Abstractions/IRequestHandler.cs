namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Base handler contract. <see cref="Sender"/> always resolves the closed generic
/// <c>IRequestHandler&lt;TRequest, TResponse&gt;</c> for the request's runtime type — it never needs
/// to know whether the concrete request is a query or a command. <see cref="IQueryHandler{TQuery,TResponse}"/>
/// and <see cref="ICommandHandler{TCommand,TResponse}"/> exist purely as semantic/namespace markers for
/// handler authors and the architecture fitness tests; interface inheritance means a type implementing
/// either of them is still discoverable here via reflection's <c>Type.GetInterfaces()</c>.
/// </summary>
/// <typeparam name="TRequest">The request type this handler processes.</typeparam>
/// <typeparam name="TResponse">The type of response the handler produces.</typeparam>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IAppRequest<TResponse>
{
    /// <summary>
    /// Handles <paramref name="request"/> and produces its response.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response for the request.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
