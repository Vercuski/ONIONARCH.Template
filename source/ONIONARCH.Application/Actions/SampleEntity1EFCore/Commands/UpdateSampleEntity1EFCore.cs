using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1EFCore.Commands;

public sealed record UpdateSampleEntity1EFCoreRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class UpdateSampleEntity1EFCoreHandler(ICommandDbContext commandDbContext,
    ILogger<UpdateSampleEntity1EFCoreHandler> logger) : IMediatRCommandHandler<UpdateSampleEntity1EFCoreRequest, int>
{
    public Task<int> Handle(
        UpdateSampleEntity1EFCoreRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            commandDbContext.Alter(request.SampleEntity);
            var result = commandDbContext.SaveChanges();
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating SampleEntity1EFCore.");
        }
        return Task.FromResult(0);
    }
}
