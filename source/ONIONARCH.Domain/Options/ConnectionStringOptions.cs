using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Domain.Options;

public sealed class ConnectionStringOptions : BaseConfig
{
    public string SampleDb { get; set; } = null!;
    public override string Section => "ConnectionStrings";
}
