using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

public sealed record UpdateSampleEntityEFCoreRequest(SampleEntityDefinition SampleEntity)
    : IMediatRCommandRequest<Result<int>>;
internal sealed class UpdateSampleEntityEFCoreHandler(ICommandDbContext commandDbContext,
    ILogger<UpdateSampleEntityEFCoreHandler> logger)
    : IMediatRCommandHandler<UpdateSampleEntityEFCoreRequest, Result<int>>
{
    public Task<Result<int>> Handle(
        UpdateSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected;
        try
        {
            commandDbContext.Alter(request.SampleEntity);
            rowsAffected = commandDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating SampleEntityEFCore.");
            return Task.FromResult(Result<int>.Failure("Error updating SampleEntityEFCore.", ResultErrorType.Validation));
        }
        return Task.FromResult(Result<int>.Success(rowsAffected));
    }
}
