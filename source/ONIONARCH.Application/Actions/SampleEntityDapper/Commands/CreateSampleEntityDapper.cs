using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Repositories;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

/// <summary>
/// Command to insert a new sample entity through the Dapper persistence path.
/// </summary>
/// <param name="SampleEntity">The entity to insert.</param>
public sealed record CreateSampleEntityDapperRequest(SampleEntityDefinition SampleEntity)
    : ICommandRequest<Result<int>>;

/// <summary>
/// Handles <see cref="CreateSampleEntityDapperRequest"/> by delegating to the Dapper command repository.
/// </summary>
/// <param name="repository">The write-side Dapper repository port.</param>
internal sealed class CreateSampleEntityDapperHandler(ISampleEntityDapperCommandRepository repository)
    : ICommandHandler<CreateSampleEntityDapperRequest, Result<int>>
{
    /// <summary>
    /// Inserts the requested entity.
    /// </summary>
    /// <param name="request">The command carrying the entity to insert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A successful result containing the number of rows affected.</returns>
    public async Task<Result<int>> Handle(
        CreateSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        var rowsAffected = await repository.CreateAsync(request.SampleEntity, cancellationToken);
        return Result<int>.Success(rowsAffected);
    }
}
