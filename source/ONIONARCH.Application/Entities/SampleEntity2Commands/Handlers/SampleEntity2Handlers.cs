using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Entities.SampleEntity2Commands.Requests;

namespace ONIONARCH.Application.Entities.SampleEntity2Commands.Handlers;

internal sealed class CreateSampleEntity2Handler(ICommandDbContext dbContext,
    ILogger<CreateSampleEntity2Handler> logger) : ICommandHandler<CreateSampleEntity2Request, int>
{
    private readonly ICommandDbContext _dbContext = dbContext;

    public Task<int> Handle(
        CreateSampleEntity2Request request,
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
            logger.LogError(ex, "Error creating sampleentity2.");
        }
        return Task.FromResult(0);
    }
}

internal sealed class DeleteSampleEntity2Handler(ICommandDbContext dbContext,
    ILogger<DeleteSampleEntity2Handler> logger) : ICommandHandler<DeleteSampleEntity2Request, int>
{
    private readonly ICommandDbContext _dbContext = dbContext;

    public Task<int> Handle(
        DeleteSampleEntity2Request request,
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
            logger.LogError(ex, "Error deleting sampleentity2.");
        }
        return Task.FromResult(0);
    }
}

internal sealed class UpdateSampleEntity2Handler(ICommandDbContext dbContext,
    ILogger<UpdateSampleEntity2Handler> logger) : ICommandHandler<UpdateSampleEntity2Request, int>
{
    private readonly ICommandDbContext _dbContext = dbContext;

    public Task<int> Handle(
        UpdateSampleEntity2Request request,
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
            logger.LogError(ex, "Error updating sampleentity2.");
        }
        return Task.FromResult(0);
    }
}
