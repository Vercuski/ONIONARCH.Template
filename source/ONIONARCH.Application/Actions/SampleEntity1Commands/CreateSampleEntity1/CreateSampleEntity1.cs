using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Commands.CreateSampleEntity1;
public sealed record CreateSampleEntity1Request(SampleEntity1 SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class CreateSampleEntity1Handler(ICommandDbContext commandDbContext,
    ILogger<CreateSampleEntity1Handler> logger) : IMediatRCommandHandler<CreateSampleEntity1Request, int>
{
    public Task<int> Handle(
        CreateSampleEntity1Request request,
        CancellationToken cancellationToken)
    {
        try
        {
            commandDbContext.Insert(request.SampleEntity);
            var result = commandDbContext.SaveChanges();
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating sampleentity1.");
        }
        return Task.FromResult(0);
    }
}
