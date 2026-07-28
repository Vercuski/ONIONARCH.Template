using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Queries;

public sealed class GetMultipleSampleEntityEFCoresRequest
    : IMediatRQueryRequest<Result<List<SampleEntityDefinition>>>;
internal sealed class GetMultipleSampleEntityEFCoresHandler(IQueryDbContext queryDbContext,
    ILogger<GetMultipleSampleEntityEFCoresHandler> logger
    )
    : IMediatRQueryHandler<GetMultipleSampleEntityEFCoresRequest, Result<List<SampleEntityDefinition>>>
{
    public async Task<Result<List<SampleEntityDefinition> >> Handle(
        GetMultipleSampleEntityEFCoresRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            List<SampleEntityDefinition> response = await queryDbContext.ToListAsync(
                queryDbContext.Set<SampleEntityDefinition>(), cancellationToken);
            return Result<List<SampleEntityDefinition>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving SampleEntityEFCore.");
            return Result<List<SampleEntityDefinition>>.Failure("Error retrieving SampleEntityEFCore.", ResultErrorType.Validation);
        }
    }
}
