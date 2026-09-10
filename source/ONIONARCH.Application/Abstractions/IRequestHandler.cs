namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Base handler contract. <see cref="Sender"/> always resolves the closed generic
/// <c>IRequestHandler&lt;TRequest, TResponse&gt;</c> for the request's runtime type — it never needs
/// to know whether the concrete request is a query or a command. <see cref="IQueryHandler{TQuery,TResponse}"/>
/// and <see cref="ICommandHandler{TCommand,TResponse}"/> exist purely as semantic/namespace markers for
/// handler authors and the architecture fitness tests; interface inheritance means a type implementing
/// either of them is still discoverable here via reflection's <c>Type.GetInterfaces()</c>.
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IAppRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
