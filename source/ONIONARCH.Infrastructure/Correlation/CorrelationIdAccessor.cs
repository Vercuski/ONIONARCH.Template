namespace ONIONARCH.Infrastructure.Correlation;

/// <summary>
/// Ambient accessor for the current operation's correlation ID, backed by AsyncLocal so the
/// value flows implicitly through the entire async call chain of a single request — every
/// downstream await, request handler, and repository call sees it without it being passed
/// as a parameter anywhere.
/// </summary>
/// <remarks>
/// Registered as a singleton. That is safe because the backing <see cref="AsyncLocal{T}"/> is
/// static and isolates its value per async execution flow, so concurrent requests never see
/// each other's IDs.
/// </remarks>
public sealed class CorrelationIdAccessor
{
    /// <summary>
    /// Holds the correlation ID for the current async execution flow.
    /// </summary>
    private static readonly AsyncLocal<string?> Current = new();

    /// <summary>
    /// Gets the correlation ID for the current async flow, or <see cref="string.Empty"/> if none has been set.
    /// </summary>
    public string CorrelationId => Current.Value ?? string.Empty;

    /// <summary>
    /// Sets the correlation ID for the current async flow and every flow it subsequently spawns.
    /// </summary>
    /// <param name="correlationId">The correlation ID to make ambient.</param>
    public void Set(string correlationId)
    {
        Current.Value = correlationId;
    }
}
