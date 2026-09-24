namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Handles a read-only <see cref="IQueryRequest{TResponse}"/>.
/// </summary>
/// <typeparam name="TQuery">The query type this handler processes.</typeparam>
/// <typeparam name="TResponse">The type of response the query produces.</typeparam>
/// <remarks>
/// Adds no members to <see cref="IRequestHandler{TRequest,TResponse}"/>; it is a semantic marker
/// that the architecture fitness tests use to require query handlers to be sealed and to take a
/// query-side persistence abstraction in their constructor.
/// </remarks>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQueryRequest<TResponse>;
