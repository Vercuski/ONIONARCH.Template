using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Domain.Options;

public sealed record ConnectionStringOptions : BaseOptionsConfig
{
    public string SampleDb { get; set; } = null!;
    public override string Section => "ConnectionStrings";
}
