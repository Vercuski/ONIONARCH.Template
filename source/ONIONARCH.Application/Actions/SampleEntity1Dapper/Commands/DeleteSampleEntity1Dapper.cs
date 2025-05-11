using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Dapper.Commands;
public sealed record DeleteSampleEntity1DapperRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class DeleteSampleEntity1DapperHandler(ICommandDbContext commandDbContext,
    ILogger<DeleteSampleEntity1DapperHandler> logger) : IMediatRCommandHandler<DeleteSampleEntity1DapperRequest, int>
{
    public Task<int> Handle(
        DeleteSampleEntity1DapperRequest request,
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
            logger.LogError(ex, "Error deleting SampleEntity1Dapper.");
        }
        return Task.FromResult(0);
    }
}
