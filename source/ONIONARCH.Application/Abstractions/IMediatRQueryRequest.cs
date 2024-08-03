using MediatR;

namespace ONIONARCH.Application.Abstractions;

public interface IMediatRQueryRequest<out TResponse> : IRequest<TResponse>;