using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;

namespace ONIONARCH.Application.Entities.SampleEntity1Commands.DeleteSampleEntity1;

internal sealed class DeleteSampleEntity1Handler(ICommandDbContext dbContext,
    ILogger<DeleteSampleEntity1Handler> logger) : ICommandHandler<DeleteSampleEntity1Request, int>
{
    private readonly ICommandDbContext _dbContext = dbContext;

    public Task<int> Handle(
        DeleteSampleEntity1Request request,
        CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Remove(request.SampleEntity);
            var result = _dbContext.SaveChanges();
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting sampleentity1.");
        }
        return Task.FromResult(0);
    }
}
