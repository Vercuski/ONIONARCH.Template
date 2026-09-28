using ONIONARCH.Persistence.Providers;

namespace ONIONARCH.Persistence.SqlServer;

/// <summary>
/// Composition-root extension that opts a host in to the Microsoft SQL Server persistence provider.
/// This is the project's only public surface.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the Microsoft SQL Server provider. It is selected at startup when the <c>DatabasePlatform</c>
    /// configuration's <c>QueryDbPlatform</c> or <c>CommandDbPlatform</c> is <c>MSSQL</c>
    /// (case-insensitive).
    /// </summary>
    /// <param name="registry">The registry passed to <c>AddPersistenceRegistrations</c>.</param>
    /// <returns>The same <paramref name="registry"/>, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="registry"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">A MSSQL provider is already registered.</exception>
    public static DatabaseProviderRegistry AddSqlServer(this DatabaseProviderRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        return registry.Add(new SqlServerDatabaseProvider());
    }
}
