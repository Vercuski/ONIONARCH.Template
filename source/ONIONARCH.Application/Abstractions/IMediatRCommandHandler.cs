using MediatR;

namespace ONIONARCH.Application.Abstractions;

public interface IMediatRCommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : IMediatRCommandRequest<TResponse>;