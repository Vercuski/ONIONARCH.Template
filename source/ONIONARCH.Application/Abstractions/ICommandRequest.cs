namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Marker for a CQRS command — a request that changes system state. Handled by an
/// <see cref="ICommandHandler{TCommand,TResponse}"/>.
/// </summary>
/// <typeparam name="TResponse">The type of response the command produces.</typeparam>
public interface ICommandRequest<out TResponse> : IAppRequest<TResponse>;
