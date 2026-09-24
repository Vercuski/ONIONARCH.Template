using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using ONIONARCH.Infrastructure.Correlation;

namespace ONIONARCH.Tests.InfrastructureTests;

/// <summary>
/// Unit tests for <see cref="CorrelationIdMiddleware"/> in isolation, using a
/// <see cref="DefaultHttpContext"/> and a stub next delegate.
/// </summary>
[TestFixture]
public class CorrelationIdMiddlewareTests
{
    /// <summary>
    /// Verifies that, with no incoming header, the middleware generates a GUID, writes it to the
    /// response header, and makes the same value visible to downstream code via
    /// <see cref="CorrelationIdAccessor"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task InvokeAsync_Should_SetResponseHeader_AndMakeIdAvailableDownstream()
    {
        var accessor = new CorrelationIdAccessor();
        string? observedInsideNext = null;

        // The lambda below stands in for "the rest of the pipeline" — a request handler or
        // repository call several layers deep. It reads the accessor with no parameter having
        // been passed to it, which is the "invisible" propagation this middleware exists for.
        var middleware = new CorrelationIdMiddleware(context =>
        {
            observedInsideNext = accessor.CorrelationId;
            return Task.CompletedTask;
        });

        var httpContext = new DefaultHttpContext();

        await middleware.InvokeAsync(httpContext, accessor, NullLogger<CorrelationIdMiddleware>.Instance);

        var headerValue = httpContext.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(headerValue, Is.Not.Empty);
            Assert.That(Guid.TryParse(headerValue, out _), Is.True);
            Assert.That(observedInsideNext, Is.EqualTo(headerValue));
        }
    }

    /// <summary>
    /// Verifies that an incoming correlation ID header is reused and echoed back unchanged.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task InvokeAsync_Should_ReuseIncomingCorrelationIdHeader_WhenPresent()
    {
        var accessor = new CorrelationIdAccessor();
        var httpContext = new DefaultHttpContext();
        var incomingId = Guid.NewGuid().ToString();
        httpContext.Request.Headers[CorrelationIdMiddleware.HeaderName] = incomingId;

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(httpContext, accessor, NullLogger<CorrelationIdMiddleware>.Instance);

        var headerValue = httpContext.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();
        Assert.That(headerValue, Is.EqualTo(incomingId));
    }
}
