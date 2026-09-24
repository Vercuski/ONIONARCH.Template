namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Base marker for anything dispatchable through <see cref="ISender"/>. Deliberately carries no
/// members — it exists purely so <see cref="ISender.Send{TResponse}"/> and
/// <see cref="IPipelineBehavior{TRequest,TResponse}"/> have a single covariant type to close
/// their generics over, regardless of whether the concrete request is a query or a command.
/// </summary>
/// <typeparam name="TResponse">The type of response the request's handler produces.</typeparam>
/// <remarks>
/// Request types should implement <see cref="IQueryRequest{TResponse}"/> or
/// <see cref="ICommandRequest{TResponse}"/> rather than this interface directly.
/// </remarks>
public interface IAppRequest<out TResponse>;
