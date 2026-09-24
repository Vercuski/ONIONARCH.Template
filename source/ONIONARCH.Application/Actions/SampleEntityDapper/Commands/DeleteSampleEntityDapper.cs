using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Repositories;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

/// <summary>
/// Command to delete a sample entity by key through the Dapper persistence path.
/// </summary>
/// <param name="SampleId">The key of the entity to delete.</param>
public sealed record DeleteSampleEntityDapperRequest(int SampleId)
    : ICommandRequest<Result<int>>;

/// <summary>
/// Handles <see cref="DeleteSampleEntityDapperRequest"/> by delegating to the Dapper command repository.
/// </summary>
/// <param name="repository">The write-side Dapper repository port.</param>
internal sealed class DeleteSampleEntityDapperHandler(ISampleEntityDapperCommandRepository repository)
    : ICommandHandler<DeleteSampleEntityDapperRequest, Result<int>>
{
    /// <summary>
    /// Deletes the entity with the requested key.
    /// </summary>
    /// <param name="request">The command carrying the key to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A successful result containing the number of rows affected (0 if no row matched —
    /// a missing row is not reported as a failure).
    /// </returns>
    public async Task<Result<int>> Handle(
        DeleteSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        var rowsAffected = await repository.DeleteAsync(request.SampleId, cancellationToken);
        return Result<int>.Success(rowsAffected);
    }
}
