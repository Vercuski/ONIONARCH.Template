using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

public sealed record DeleteSampleEntityEFCoreRequest(SampleEntityDefinition Entity) : IMediatRCommandRequest<int>;
internal sealed class DeleteSampleEntityEFCoreHandler(ICommandDbContext commandDbContext,
    ILogger<DeleteSampleEntityEFCoreHandler> logger) : IMediatRCommandHandler<DeleteSampleEntityEFCoreRequest, int>
{
    public Task<int> Handle(
        DeleteSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected = 0;
        try
        {
            commandDbContext.Delete(request.Entity);
            rowsAffected = commandDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SampleEntityEFCore.");
        }
        return Task.FromResult(rowsAffected);
    }
}
