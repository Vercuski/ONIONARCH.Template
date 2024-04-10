using ONIONARCH.Domain.Abstractions;
using System.ComponentModel;

namespace ONIONARCH.Domain.Options;

public sealed record SwaggerOptions : BaseOptionsConfig
{
    public string ServerUrl { get; set; } = null!;
    public override string Section => "Swagger";
}
