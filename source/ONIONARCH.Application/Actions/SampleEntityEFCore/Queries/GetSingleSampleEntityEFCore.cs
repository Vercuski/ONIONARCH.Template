using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Queries;

public sealed record GetSingleSampleEntityEFCoreRequest(int Id)
    : IMediatRQueryRequest<Result<SampleEntityDefinition>>;
internal sealed class GetSingleSampleEntityEFCoreHandler(
    IQueryDbContext queryDbContext,
    ILogger<GetSingleSampleEntityEFCoreHandler> logger
    ) : IMediatRQueryHandler<GetSingleSampleEntityEFCoreRequest, Result<SampleEntityDefinition>>
{
    public async Task<Result<SampleEntityDefinition>> Handle(
        GetSingleSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<SampleEntityDefinition> query = queryDbContext.Set<SampleEntityDefinition>()
                .Where(e => e.SampleId == request.Id);
            SampleEntityDefinition? response = await queryDbContext.SingleOrDefaultAsync(query, cancellationToken);
            return response is null ? Result<SampleEntityDefinition>.Failure("SampleEntityEFCore not found.", ResultErrorType.NotFound) : Result<SampleEntityDefinition>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving SampleEntityEFCore.");
            return Result<SampleEntityDefinition>.Failure("Error retrieving SampleEntityEFCore.", ResultErrorType.Validation);
        }
    }
}
