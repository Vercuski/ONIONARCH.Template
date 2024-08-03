using MediatR;

namespace ONIONARCH.Application.Abstractions;

public interface IMediatRCommandRequest<out TResponse>
    : IRequest<TResponse>;