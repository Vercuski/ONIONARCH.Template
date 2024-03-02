using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;

namespace ONIONARCH.Application.Entities.SampleEntity1Commands.UpdateSampleEntity1;

internal sealed class UpdateSampleEntity1Handler(ICommandDbContext dbContext,
    ILogger<UpdateSampleEntity1Handler> logger) : ICommandHandler<UpdateSampleEntity1Request, int>
{
    private readonly ICommandDbContext _dbContext = dbContext;

    public Task<int> Handle(
        UpdateSampleEntity1Request request,
        CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Alter(request.SampleEntity);
            var result = _dbContext.SaveChanges();
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating sampleentity1.");
        }
        return Task.FromResult(0);
    }
}
