using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ONIONARCH.Persistence.Providers;

namespace ONIONARCH.Persistence.MySql;

/// <summary>
/// Composition-root extension that opts a host in to the MySQL persistence provider.
/// This is the project's only public surface.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// The configuration key holding the MySQL/MariaDB server version EF Core targets, e.g.
    /// <c>8.4.0-mysql</c> or <c>11.4.2-mariadb</c>.
    /// </summary>
    public const string ServerVersionConfigurationKey = "DatabasePlatform:MySqlServerVersion";

    /// <summary>
    /// Registers the MySQL provider. It is selected at startup when the <c>DatabasePlatform</c>
    /// configuration's <c>QueryDbPlatform</c> or <c>CommandDbPlatform</c> is <c>MySQL</c>
    /// (case-insensitive).
    /// </summary>
    /// <remarks>
    /// The server version is read from <see cref="ServerVersionConfigurationKey"/> and validated here,
    /// so a missing or malformed value fails at startup rather than on the first database call. It is
    /// required whenever the provider is registered, even if neither side is currently configured for
    /// MySQL; remove the registration to drop the requirement. One version applies to both CQRS sides.
    /// </remarks>
    /// <param name="registry">The registry passed to <c>AddPersistenceRegistrations</c>.</param>
    /// <param name="configuration">The host configuration to read the server version from.</param>
    /// <returns>The same <paramref name="registry"/>, for chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="registry"/> or <paramref name="configuration"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The server version is missing or cannot be parsed, or a MySQL provider is already registered.
    /// </exception>
    public static DatabaseProviderRegistry AddMySql(this DatabaseProviderRegistry registry, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(configuration);

        var value = configuration[ServerVersionConfigurationKey];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"'{ServerVersionConfigurationKey}' must be set when the MySQL provider is registered " +
                "(e.g. \"8.4.0-mysql\" or \"11.4.2-mariadb\").");
        }

        if (!ServerVersion.TryParse(value, out var serverVersion))
        {
            throw new InvalidOperationException(
                $"'{ServerVersionConfigurationKey}' value '{value}' is not a valid server version " +
                "(expected e.g. \"8.4.0-mysql\" or \"11.4.2-mariadb\").");
        }

        return registry.Add(new MySqlDatabaseProvider(serverVersion));
    }
}
