using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Domain.Options;

public sealed record LogOptions : IBaseOptionsConfig
{
    public LoggingLevel LoggingLevel { get; set; } = null!;
    public string Section => "Logging";
}

public sealed record LoggingLevel : IBaseOptionsConfig
{
    public string Default { get; set; } = null!;
    public string MicrosoftAspNetCore { get; set; } = null!;
    public string Section => "Logging:LoggingLevel";
}