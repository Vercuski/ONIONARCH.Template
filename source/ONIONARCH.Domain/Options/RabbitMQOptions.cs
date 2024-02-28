using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Domain.Options;

public sealed class RabbitMQOptions : BaseConfig
{
    public string Host { get; set; } = null!;
    public string VirtualHost { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public override string Section => "RabbitMQ";
}
