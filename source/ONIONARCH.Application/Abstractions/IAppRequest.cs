namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Base marker for anything dispatchable through <see cref="ISender"/>. Deliberately carries no
/// members — it exists purely so <see cref="ISender.Send{TResponse}"/> and
/// <see cref="IPipelineBehavior{TRequest,TResponse}"/> have a single covariant type to close
/// their generics over, regardless of whether the concrete request is a query or a command.
/// </summary>
public interface IAppRequest<out TResponse>;
