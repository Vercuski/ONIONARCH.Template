﻿using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Domain.Options;

public sealed record ConnectionStringOptions : IBaseOptionsConfig
{
    public string QueryDbConnection { get; set; } = null!;
    public string CommandDbConnection { get; set; } = null!;
    public string Section => "ConnectionStrings";
}
