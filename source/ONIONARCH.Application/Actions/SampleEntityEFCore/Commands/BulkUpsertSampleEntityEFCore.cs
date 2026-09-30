using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

/// <summary>
/// Command to insert or update many sample entities by key in one bulk operation through the EF Core
/// bulk port.
/// </summary>
/// <param name="SampleEntities">
/// The entities to write. An entity whose key matches an existing row updates it; one with an unsaved
/// key (<c>0</c>) is inserted.
/// </param>
public sealed record BulkUpsertSampleEntityEFCoreRequest(IReadOnlyCollection<SampleEntityDefinition> SampleEntities)
    : ICommandRequest<Result<int>>;

/// <summary>
/// Handles <see cref="BulkUpsertSampleEntityEFCoreRequest"/> using the EF Core bulk command port.
/// </summary>
/// <param name="bulkCommandDbContext">The write-side EF Core bulk port.</param>
internal sealed class BulkUpsertSampleEntityEFCoreHandler(IBulkCommandDbContext bulkCommandDbContext)
    : ICommandHandler<BulkUpsertSampleEntityEFCoreRequest, Result<int>>
{
    /// <summary>
    /// Inserts or updates the requested entities by primary key.
    /// </summary>
    /// <param name="request">The command carrying the entities to write.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A successful result containing the number of entities written, or a validation failure if the
    /// request carries no entities.
    /// </returns>
    /// <exception cref="NotSupportedException">The command database platform does not support bulk upserts.</exception>
    public async Task<Result<int>> Handle(
        BulkUpsertSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        if (request.SampleEntities is not { Count: > 0 })
        {
            return Result<int>.Failure("At least one sample entity is required.", ResultErrorType.Validation);
        }

        await bulkCommandDbContext.BulkUpsertAsync(request.SampleEntities, cancellationToken: cancellationToken);
        return Result<int>.Success(request.SampleEntities.Count);
    }
}
