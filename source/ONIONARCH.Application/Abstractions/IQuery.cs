using MediatR;

namespace ONIONARCH.Application.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}