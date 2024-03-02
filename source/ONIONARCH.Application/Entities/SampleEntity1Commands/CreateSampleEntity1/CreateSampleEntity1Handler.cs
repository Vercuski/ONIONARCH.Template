using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;

namespace ONIONARCH.Application.Entities.SampleEntity1Commands.CreateSampleEntity1;

internal sealed class CreateSampleEntity1Handler(ICommandDbContext dbContext,
    ILogger<CreateSampleEntity1Handler> logger) : ICommandHandler<CreateSampleEntity1Request, int>
{
    private readonly ICommandDbContext _dbContext = dbContext;

    public Task<int> Handle(
        CreateSampleEntity1Request request,
        CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Insert(request.SampleEntity);
            var result = _dbContext.SaveChanges();
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating sampleentity1.");
        }
        return Task.FromResult(0);
    }
}
