using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

public sealed record CreateSampleEntityEFCoreRequest(SampleEntityDefinition SampleEntity)
    : IMediatRCommandRequest<Result<int>>;
internal sealed class CreateSampleEntityEFCoreHandler(ICommandDbContext commandDbContext,
    ILogger<CreateSampleEntityEFCoreHandler> logger)
    : IMediatRCommandHandler<CreateSampleEntityEFCoreRequest, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected;
        try
        {
            commandDbContext.Insert(request.SampleEntity);
            rowsAffected = commandDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating SampleEntityEFCore.");
            return Result<int>.Failure("Error creating SampleEntityEFCore.", ResultErrorType.Validation);
        }
        return Result<int>.Success(rowsAffected);
    }
}
