namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Marker for a CQRS query — a request that reads data without changing system state. Handled by an
/// <see cref="IQueryHandler{TQuery,TResponse}"/>.
/// </summary>
/// <typeparam name="TResponse">The type of response the query produces.</typeparam>
public interface IQueryRequest<out TResponse> : IAppRequest<TResponse>;
