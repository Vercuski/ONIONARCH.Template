namespace ONIONARCH.Application.Abstractions;

public interface ICommandRequest<out TResponse> : IAppRequest<TResponse>;
