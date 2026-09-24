namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Cross-cutting wrapper around a request/handler pair, resolved and chained by <see cref="Sender"/>
/// for every dispatched request regardless of its concrete type. Mirrors MediatR's
/// IPipelineBehavior&lt;TRequest,TResponse&gt; shape closely enough that behaviors written for it (see
/// <see cref="ONIONARCH.Application.Behaviors.LoggingBehavior{TRequest,TResponse}"/>) only need their
/// <c>next</c> delegate signature updated.
/// </summary>
/// <typeparam name="TRequest">The request type being dispatched.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request's handler.</typeparam>
/// <remarks>
/// Register open-generic implementations against <c>IPipelineBehavior&lt;,&gt;</c>; behaviors run in
/// registration order, with the first-registered behavior outermost.
/// </remarks>
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAppRequest<TResponse>
{
    /// <summary>
    /// Executes this behavior's logic around the rest of the pipeline.
    /// </summary>
    /// <param name="request">The request being dispatched.</param>
    /// <param name="next">
    /// Invokes the next behavior in the chain, or the request handler if this is the innermost
    /// behavior. A behavior may short-circuit by not calling it.
    /// </param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response produced by <paramref name="next"/>, or a substitute response.</returns>
    Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken);
}
