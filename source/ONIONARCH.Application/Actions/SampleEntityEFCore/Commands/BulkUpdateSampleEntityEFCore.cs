using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

/// <summary>
/// Command to update many sample entities with one set-based <c>UPDATE</c> through the EF Core bulk
/// port (<see cref="IBulkCommandDbContext.UpdateWhereAsync{TEntity}"/>), without loading them. Works on
/// every database platform.
/// </summary>
/// <param name="SampleIds">The keys of the entities to update.</param>
/// <param name="SampleBoolean">The value to assign to <see cref="SampleEntityDefinition.SampleBoolean"/>.</param>
/// <param name="SampleIntIncrement">The amount to add to each entity's current <see cref="SampleEntityDefinition.SampleInt"/>.</param>
public sealed record BulkUpdateSampleEntityEFCoreRequest(
    IReadOnlyCollection<int> SampleIds,
    bool SampleBoolean,
    int SampleIntIncrement)
    : ICommandRequest<Result<int>>;

/// <summary>
/// Handles <see cref="BulkUpdateSampleEntityEFCoreRequest"/> using the EF Core bulk command port.
/// </summary>
/// <param name="bulkCommandDbContext">The write-side EF Core bulk port.</param>
internal sealed class BulkUpdateSampleEntityEFCoreHandler(IBulkCommandDbContext bulkCommandDbContext)
    : ICommandHandler<BulkUpdateSampleEntityEFCoreRequest, Result<int>>
{
    /// <summary>
    /// Sets <see cref="SampleEntityDefinition.SampleBoolean"/> to a constant and increments
    /// <see cref="SampleEntityDefinition.SampleInt"/> relative to its current value, for every
    /// requested key, in a single statement. Demonstrates both kinds of setter.
    /// </summary>
    /// <param name="request">The command carrying the keys and new values.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A successful result containing the number of rows updated (keys that match no row are
    /// ignored), or a validation failure if the request carries no keys.
    /// </returns>
    public async Task<Result<int>> Handle(
        BulkUpdateSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        if (request.SampleIds is not { Count: > 0 })
        {
            return Result<int>.Failure("At least one sample id is required.", ResultErrorType.Validation);
        }

        var sampleIds = request.SampleIds;
        int increment = request.SampleIntIncrement;
        int rowsUpdated = await bulkCommandDbContext.UpdateWhereAsync<SampleEntityDefinition>(
            e => sampleIds.Contains(e.SampleId),
            set => set
                .Set(e => e.SampleBoolean, request.SampleBoolean)
                .Set(e => e.SampleInt, e => e.SampleInt + increment),
            cancellationToken);

        return Result<int>.Success(rowsUpdated);
    }
}
