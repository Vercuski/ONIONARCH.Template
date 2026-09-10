namespace ONIONARCH.Application.Abstractions;

public interface IQueryRequest<out TResponse> : IAppRequest<TResponse>;
