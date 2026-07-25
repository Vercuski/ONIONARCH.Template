using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

public sealed record UpdateSampleEntityEFCoreRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class UpdateSampleEntityEFCoreHandler(ICommandDbContext commandDbContext,
    ILogger<UpdateSampleEntityEFCoreHandler> logger) : IMediatRCommandHandler<UpdateSampleEntityEFCoreRequest, int>
{
    public Task<int> Handle(
        UpdateSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected = 0;
        try
        {
            commandDbContext.Alter(request.SampleEntity);
            rowsAffected = commandDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating SampleEntityEFCore.");
        }
        return Task.FromResult(rowsAffected);
    }
}
