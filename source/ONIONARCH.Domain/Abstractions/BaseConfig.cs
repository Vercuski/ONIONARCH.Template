namespace ONIONARCH.Domain.Abstractions;

public abstract record BaseConfig
{
    public abstract string Section { get; }
}
