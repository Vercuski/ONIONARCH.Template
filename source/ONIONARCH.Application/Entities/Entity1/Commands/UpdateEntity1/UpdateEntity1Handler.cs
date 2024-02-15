using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ONIONARCH.Application.Entities.Entity1.Commands.UpdateEntity1;

internal sealed class UpdateEntity1Handler(ICommandDbContext dbContext,
    ILogger<UpdateEntity1Handler> logger) : ICommandHandler<UpdateEntity1Request, int>
{
    private readonly ICommandDbContext _dbContext = dbContext;

    public Task<int> Handle(
        UpdateEntity1Request request,
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
            logger.LogError(ex, "Error updating entity1.");
        }
        return Task.FromResult(0);
    }
}
