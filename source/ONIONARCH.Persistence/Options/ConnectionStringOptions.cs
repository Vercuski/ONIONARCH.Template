using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Persistence.Options;

public sealed record ConnectionStringOptions : IBaseOptionsConfig
{
    public string QueryDbConnection { get; set; } = null!;
    public string CommandDbConnection { get; set; } = null!;
    public string Section => "ConnectionStrings";
}
