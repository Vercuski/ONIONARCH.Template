using ONIONARCH.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace ONIONARCH.Application.Entities.Entity1.Commands.CreateEntity1;

internal sealed class CreateEntity1Handler(ICommandDbContext dbContext,
    ILogger<CreateEntity1Handler> logger) : ICommandHandler<CreateEntity1Request, int>
{
    private readonly ICommandDbContext _dbContext = dbContext;

    public Task<int> Handle(
        CreateEntity1Request request,
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
            logger.LogError(ex, "Error creating entity.");
        }
        return Task.FromResult(0);
    }
}
