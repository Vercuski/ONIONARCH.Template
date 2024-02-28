using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;

namespace ONIONARCH.Application.Entities.Entity1.Commands.DeleteEntity1;

internal sealed class DeleteEntity1Handler(ICommandDbContext dbContext,
    ILogger<DeleteEntity1Handler> logger) : ICommandHandler<DeleteEntity1Request, int>
{
    private readonly ICommandDbContext _dbContext = dbContext;

    public Task<int> Handle(
        DeleteEntity1Request request,
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
            logger.LogError(ex, "Error deleting entity.");
        }
        return Task.FromResult(0);
    }
}
