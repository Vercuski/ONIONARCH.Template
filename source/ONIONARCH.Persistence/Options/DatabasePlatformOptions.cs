using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Persistence.Options;

/// <summary>
/// Selects the database platform for each side of the CQRS split, bound from the
/// <c>DatabasePlatform</c> configuration section. Supported values (case-insensitive):
/// <c>MSSQL</c>, <c>POSTGRESQL</c>, <c>MYSQL</c>.
/// </summary>
public sealed record DatabasePlatformOptions : IBaseOptionsConfig
{
    /// <summary>
    /// Gets or sets the platform of the read (query) database.
    /// </summary>
    public string QueryDbPlatform { get; set; } = null!;

    /// <summary>
    /// Gets or sets the platform of the write (command) database.
    /// </summary>
    public string CommandDbPlatform { get; set; } = null!;

    /// <inheritdoc />
    public string Section => "DatabasePlatform";
}