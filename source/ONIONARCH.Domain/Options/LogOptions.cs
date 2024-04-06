using ONIONARCH.Domain.Abstractions;
using System.ComponentModel;

namespace ONIONARCH.Domain.Options;

public sealed record LogOptions : BaseOptionsConfig
{
    public LogLevel LogLevel { get; set; } = null!;
    public override string Section => "Logging";
}

public sealed record LogLevel : BaseOptionsConfig
{
    public string Default { get; set; } = null!;
    public string MicrosoftAspNetCore { get; set; } = null!;
    public override string Section => "Logging:LogLevel";
}