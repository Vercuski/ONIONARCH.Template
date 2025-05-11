using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1EFCore.Commands;
public sealed record DeleteSampleEntity1EFCoreRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class DeleteSampleEntity1EFCoreHandler(ICommandDbContext commandDbContext,
    ILogger<DeleteSampleEntity1EFCoreHandler> logger) : IMediatRCommandHandler<DeleteSampleEntity1EFCoreRequest, int>
{
    public Task<int> Handle(
        DeleteSampleEntity1EFCoreRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            commandDbContext.Remove(request.SampleEntity);
            var result = commandDbContext.SaveChanges();
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SampleEntity1EFCore.");
        }
        return Task.FromResult(0);
    }
}
