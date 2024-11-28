using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Commands.UpdateSampleEntity1;

public sealed record UpdateSampleEntity1Request(SampleEntity1 SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class UpdateSampleEntity1Handler(ICommandDbContext commandDbContext,
    ILogger<UpdateSampleEntity1Handler> logger) : IMediatRCommandHandler<UpdateSampleEntity1Request, int>
{
    public Task<int> Handle(
        UpdateSampleEntity1Request request,
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
            logger.LogError(ex, "Error updating sampleentity1.");
        }
        return Task.FromResult(0);
    }
}
