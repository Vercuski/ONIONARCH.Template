namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Cross-cutting wrapper around a request/handler pair, resolved and chained by <see cref="Sender"/>
/// for every dispatched request regardless of its concrete type. Mirrors MediatR's
/// IPipelineBehavior&lt;TRequest,TResponse&gt; shape closely enough that existing behaviors (see
/// <see cref="ONIONARCH.Application.Behaviors.LoggingBehavior{TRequest,TResponse}"/>) only need their
/// <c>next</c> delegate signature updated.
/// </summary>
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAppRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken);
}
