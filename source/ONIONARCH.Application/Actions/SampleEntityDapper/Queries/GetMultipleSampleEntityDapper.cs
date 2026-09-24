using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Repositories;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Queries;

/// <summary>
/// Query to retrieve every sample entity through the Dapper persistence path.
/// </summary>
public sealed class GetMultipleSampleEntityDappersRequest
    : IQueryRequest<Result<List<SampleEntityDefinition>?>>;

/// <summary>
/// Handles <see cref="GetMultipleSampleEntityDappersRequest"/> by delegating to the Dapper query repository.
/// </summary>
/// <param name="repository">The read-side Dapper repository port.</param>
internal sealed class GetMultipleSampleEntityDappersHandler(
    ISampleEntityDapperQueryRepository repository)
    : IQueryHandler<GetMultipleSampleEntityDappersRequest, Result<List<SampleEntityDefinition>?>>
{
    /// <summary>
    /// Retrieves all sample entities.
    /// </summary>
    /// <param name="request">The query (carries no parameters).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A successful result containing every entity (empty list if none exist).</returns>
    public async Task<Result<List<SampleEntityDefinition>?>> Handle(
        GetMultipleSampleEntityDappersRequest request,
        CancellationToken cancellationToken)
    {
        var response = await repository.GetAllAsync(cancellationToken);
        return Result<List<SampleEntityDefinition>?>.Success(response);
    }
}
