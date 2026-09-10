namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Single dispatch chokepoint for the CQRS pipeline. Presentation depends only on this interface
/// and on request/response contract types — never on a concrete handler — which is the same
/// decoupling MediatR's IMediator previously provided.
/// </summary>
public interface ISender
{
    Task<TResponse> Send<TResponse>(IAppRequest<TResponse> request, CancellationToken cancellationToken = default);
}
