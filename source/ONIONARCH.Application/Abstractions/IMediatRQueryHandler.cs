﻿using MediatR;

namespace ONIONARCH.Application.Abstractions;

public interface IMediatRQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IMediatRQueryRequest<TResponse>;