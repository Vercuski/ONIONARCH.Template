using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Queries;

/// <summary>
/// Query to retrieve every sample entity through the EF Core persistence path.
/// </summary>
public sealed class GetMultipleSampleEntityEFCoresRequest
    : IQueryRequest<Result<List<SampleEntityDefinition>>>;

/// <summary>
/// Handles <see cref="GetMultipleSampleEntityEFCoresRequest"/> using the EF Core query context.
/// </summary>
/// <param name="queryDbContext">The read-side EF Core context port.</param>
internal sealed class GetMultipleSampleEntityEFCoresHandler(IQueryDbContext queryDbContext)
    : IQueryHandler<GetMultipleSampleEntityEFCoresRequest, Result<List<SampleEntityDefinition>>>
{
    /// <summary>
    /// Retrieves all sample entities.
    /// </summary>
    /// <param name="request">The query (carries no parameters).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A successful result containing every entity (empty list if none exist).</returns>
    public async Task<Result<List<SampleEntityDefinition>>> Handle(
        GetMultipleSampleEntityEFCoresRequest request,
        CancellationToken cancellationToken)
    {
        List<SampleEntityDefinition> response = await queryDbContext.ToListAsync(
            queryDbContext.Set<SampleEntityDefinition>(), cancellationToken);
        return Result<List<SampleEntityDefinition>>.Success(response);
    }
}
