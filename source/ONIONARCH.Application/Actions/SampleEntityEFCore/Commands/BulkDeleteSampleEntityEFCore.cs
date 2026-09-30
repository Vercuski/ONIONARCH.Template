using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

/// <summary>
/// Command to delete many sample entities with one set-based <c>DELETE</c> through the EF Core bulk
/// port (<see cref="IBulkCommandDbContext.DeleteWhereAsync{TEntity}"/>), without loading them. Works on
/// every database platform.
/// </summary>
/// <param name="SampleIds">The keys of the entities to delete.</param>
public sealed record BulkDeleteSampleEntityEFCoreRequest(IReadOnlyCollection<int> SampleIds)
    : ICommandRequest<Result<int>>;

/// <summary>
/// Handles <see cref="BulkDeleteSampleEntityEFCoreRequest"/> using the EF Core bulk command port.
/// </summary>
/// <param name="bulkCommandDbContext">The write-side EF Core bulk port.</param>
internal sealed class BulkDeleteSampleEntityEFCoreHandler(IBulkCommandDbContext bulkCommandDbContext)
    : ICommandHandler<BulkDeleteSampleEntityEFCoreRequest, Result<int>>
{
    /// <summary>
    /// Deletes every entity whose key is in the request, in a single statement.
    /// </summary>
    /// <param name="request">The command carrying the keys to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A successful result containing the number of rows deleted (keys that match no row are
    /// ignored), or a validation failure if the request carries no keys.
    /// </returns>
    public async Task<Result<int>> Handle(
        BulkDeleteSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        if (request.SampleIds is not { Count: > 0 })
        {
            return Result<int>.Failure("At least one sample id is required.", ResultErrorType.Validation);
        }

        var sampleIds = request.SampleIds;
        int rowsDeleted = await bulkCommandDbContext.DeleteWhereAsync<SampleEntityDefinition>(
            e => sampleIds.Contains(e.SampleId),
            cancellationToken);

        return Result<int>.Success(rowsDeleted);
    }
}
