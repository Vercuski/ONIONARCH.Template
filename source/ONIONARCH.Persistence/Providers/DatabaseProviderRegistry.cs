namespace ONIONARCH.Persistence.Providers;

/// <summary>
/// Composition-root registry of the database providers a host has opted into. Each provider project
/// contributes an extension method (<c>AddSqlServer</c>, <c>AddPostgreSql</c>, <c>AddMySql</c>) that
/// adds its provider here, so the core Persistence project resolves providers by configured platform
/// key without a compile-time dependency on any of them.
/// </summary>
public sealed class DatabaseProviderRegistry
{
    /// <summary>
    /// Registered providers keyed by <see cref="IDatabaseProvider.Platform"/>, compared case-insensitively.
    /// </summary>
    private readonly Dictionary<string, IDatabaseProvider> _providers = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The platform keys of every registered provider.
    /// </summary>
    public IReadOnlyCollection<string> Platforms => _providers.Keys;

    /// <summary>
    /// Registers <paramref name="provider"/> under its <see cref="IDatabaseProvider.Platform"/> key.
    /// </summary>
    /// <param name="provider">The provider to register.</param>
    /// <returns>This registry, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="provider"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// A provider is already registered for the same platform key (compared case-insensitively).
    /// </exception>
    public DatabaseProviderRegistry Add(IDatabaseProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (!_providers.TryAdd(provider.Platform, provider))
        {
            throw new InvalidOperationException(
                $"A database provider for platform '{provider.Platform}' has already been registered.");
        }

        return this;
    }

    /// <summary>
    /// Returns the provider registered for <paramref name="platform"/>.
    /// </summary>
    /// <param name="platform">The platform key from configuration; case-insensitive.</param>
    /// <returns>The registered provider.</returns>
    /// <exception cref="ArgumentException"><paramref name="platform"/> is <see langword="null"/>, empty, or whitespace.</exception>
    /// <exception cref="NotSupportedException">
    /// No provider is registered for <paramref name="platform"/>. The message lists the registered platforms.
    /// </exception>
    public IDatabaseProvider GetProvider(string platform)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(platform);

        if (_providers.TryGetValue(platform, out var provider))
        {
            return provider;
        }

        var registered = _providers.Count == 0 ? "none" : string.Join(", ", _providers.Keys);
        throw new NotSupportedException(
            $"Database platform '{platform}' is not registered. Registered platforms: {registered}.");
    }
}
