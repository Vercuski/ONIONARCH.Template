namespace ONIONARCH.Domain.Abstractions;

/// <summary>
/// Contract for strongly typed options classes that are bound from a named configuration section.
/// </summary>
/// <remarks>
/// Lets the composition root discover the configuration section for an options type without
/// hard-coding the section name at the call site — see Persistence's <c>DependencyInjection.GetSection&lt;T&gt;</c>,
/// which instantiates the options type and reads <see cref="Section"/> to locate its binding source.
/// </remarks>
public interface IBaseOptionsConfig
{
    /// <summary>
    /// Gets the name of the configuration section (e.g. in <c>appsettings.json</c>) that this
    /// options type is bound from.
    /// </summary>
    public abstract string Section { get; }
}
