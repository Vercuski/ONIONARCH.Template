using MediatR;

namespace ONIONARCH.Application.Abstractions;

public interface ICommand<out TResponse>
    : IRequest<TResponse>, IBaseCommand;