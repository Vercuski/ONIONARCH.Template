using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Domain.Options;

public sealed record SwaggerOptions : IBaseOptionsConfig
{
    public string ServerUrl { get; set; } = null!;
    public string Section => "Swagger";
}
