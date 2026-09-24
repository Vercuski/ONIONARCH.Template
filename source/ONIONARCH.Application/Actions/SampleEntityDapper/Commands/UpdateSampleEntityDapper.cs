using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Repositories;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

/// <summary>
/// Command to update an existing sample entity through the Dapper persistence path.
/// </summary>
/// <param name="SampleEntity">The entity carrying the key and new values.</param>
public sealed record UpdateSampleEntityDapperRequest(SampleEntityDefinition SampleEntity)
    : ICommandRequest<Result<int>>;

/// <summary>
/// Handles <see cref="UpdateSampleEntityDapperRequest"/> by delegating to the Dapper command repository.
/// </summary>
/// <param name="repository">The write-side Dapper repository port.</param>
internal sealed class UpdateSampleEntityDapperHandler(ISampleEntityDapperCommandRepository repository)
    : ICommandHandler<UpdateSampleEntityDapperRequest, Result<int>>
{
    /// <summary>
    /// Updates the requested entity.
    /// </summary>
    /// <param name="request">The command carrying the entity to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A successful result containing the number of rows affected (0 if no row matched —
    /// a missing row is not reported as a failure).
    /// </returns>
    public async Task<Result<int>> Handle(
        UpdateSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        var rowsAffected = await repository.UpdateAsync(request.SampleEntity, cancellationToken);
        return Result<int>.Success(rowsAffected);
    }
}
