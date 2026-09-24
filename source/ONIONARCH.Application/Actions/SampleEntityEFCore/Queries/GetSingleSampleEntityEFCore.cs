using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Queries;

/// <summary>
/// Query to retrieve a single sample entity by key through the EF Core persistence path.
/// </summary>
/// <param name="Id">The key of the entity to retrieve.</param>
public sealed record GetSingleSampleEntityEFCoreRequest(int Id)
    : IQueryRequest<Result<SampleEntityDefinition>>;

/// <summary>
/// Handles <see cref="GetSingleSampleEntityEFCoreRequest"/> using the EF Core query context.
/// </summary>
/// <param name="queryDbContext">The read-side EF Core context port.</param>
internal sealed class GetSingleSampleEntityEFCoreHandler(
    IQueryDbContext queryDbContext)
    : IQueryHandler<GetSingleSampleEntityEFCoreRequest, Result<SampleEntityDefinition>>
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
        GetSingleSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        IQueryable<SampleEntityDefinition> query = queryDbContext.Set<SampleEntityDefinition>()
            .Where(e => e.SampleId == request.Id);
        SampleEntityDefinition? response = await queryDbContext.SingleOrDefaultAsync(query, cancellationToken);
        return response is null ? Result<SampleEntityDefinition>.Failure("SampleEntityEFCore not found.", ResultErrorType.NotFound) : Result<SampleEntityDefinition>.Success(response);
    }
}
