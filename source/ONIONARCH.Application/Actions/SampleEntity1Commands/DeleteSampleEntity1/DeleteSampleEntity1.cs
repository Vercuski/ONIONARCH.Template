using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Commands.DeleteSampleEntity1;
public sealed record DeleteSampleEntity1Request(SampleEntity1 SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class DeleteSampleEntity1Handler(ICommandDbContext commandDbContext,
    ILogger<DeleteSampleEntity1Handler> logger) : IMediatRCommandHandler<DeleteSampleEntity1Request, int>
{
    public Task<int> Handle(
        DeleteSampleEntity1Request request,
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
            logger.LogError(ex, "Error deleting sampleentity1.");
        }
        return Task.FromResult(0);
    }
}
