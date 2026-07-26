using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

public sealed record DeleteSampleEntityEFCoreRequest(SampleEntityDefinition Entity)
    : IMediatRCommandRequest<Result<int>>;
internal sealed class DeleteSampleEntityEFCoreHandler(ICommandDbContext commandDbContext,
    ILogger<DeleteSampleEntityEFCoreHandler> logger)
    : IMediatRCommandHandler<DeleteSampleEntityEFCoreRequest, Result<int>>
{
    public Task<Result<int>> Handle(
        DeleteSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected;
        try
        {
            commandDbContext.Delete(request.Entity);
            rowsAffected = commandDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SampleEntityEFCore.");
            return Task.FromResult(Result<int>.Failure("Error deleting SampleEntityEFCore.", ResultErrorType.Validation));
        }
        return Task.FromResult(Result<int>.Success(rowsAffected));
    }
}
