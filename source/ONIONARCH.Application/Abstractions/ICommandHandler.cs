namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Handles a state-changing <see cref="ICommandRequest{TResponse}"/>.
/// </summary>
/// <typeparam name="TCommand">The command type this handler processes.</typeparam>
/// <typeparam name="TResponse">The type of response the command produces.</typeparam>
/// <remarks>
/// Adds no members to <see cref="IRequestHandler{TRequest,TResponse}"/>; it is a semantic marker
/// that the architecture fitness tests use to require command handlers to be sealed and to take a
/// command-side persistence abstraction in their constructor.
/// </remarks>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommandRequest<TResponse>;
