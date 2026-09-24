using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Repositories;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Queries;

/// <summary>
/// Query to retrieve a single sample entity by key through the Dapper persistence path.
/// </summary>
/// <param name="Id">The key of the entity to retrieve.</param>
public sealed record GetSingleSampleEntityDapperRequest(int Id)
    : IQueryRequest<Result<SampleEntityDefinition>>;

/// <summary>
/// Handles <see cref="GetSingleSampleEntityDapperRequest"/> by delegating to the Dapper query repository.
/// </summary>
/// <param name="repository">The read-side Dapper repository port.</param>
internal sealed class GetSingleSampleEntityDapperHandler(
    ISampleEntityDapperQueryRepository repository
    ) : IQueryHandler<GetSingleSampleEntityDapperRequest, Result<SampleEntityDefinition>>
{
    /// <summary>
    /// Retrieves the entity with the requested key.
    /// </summary>
    /// <param name="request">The query carrying the key to look up.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A successful result containing the entity, or a <see cref="ResultErrorType.NotFound"/>
    /// failure if no entity has that key.
    /// </returns>
    public async Task<Result<SampleEntityDefinition>> Handle(
        GetSingleSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        var response = await repository.GetByIdAsync(request.Id, cancellationToken);
        return response is null ? Result<SampleEntityDefinition>.Failure("SampleEntityDapper not found.", ResultErrorType.NotFound) : Result<SampleEntityDefinition>.Success(response);
    }
}
