namespace ONIONARCH.Domain.Abstractions;

/// <summary>
/// Marker interface identifying a type as a domain entity.
/// </summary>
/// <remarks>
/// Carries no members; concrete entities should derive from <see cref="Entity"/>, which
/// implements this interface, rather than implementing it directly.
/// </remarks>
public interface IEntity
{
}