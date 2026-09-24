using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Persistence.Options;

/// <summary>
/// Connection strings for the query and command databases, bound from the
/// <c>ConnectionStrings</c> configuration section.
/// </summary>
public sealed record ConnectionStringOptions : IBaseOptionsConfig
{
    /// <summary>
    /// Gets or sets the connection string for the read (query) database.
    /// </summary>
    public string QueryDbConnection { get; set; } = null!;

    /// <summary>
    /// Gets or sets the connection string for the write (command) database.
    /// </summary>
    public string CommandDbConnection { get; set; } = null!;

    /// <inheritdoc />
    public string Section => "ConnectionStrings";
}
