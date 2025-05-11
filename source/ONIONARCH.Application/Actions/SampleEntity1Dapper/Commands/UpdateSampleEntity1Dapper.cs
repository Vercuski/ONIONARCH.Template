using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Dapper.Commands;

public sealed record UpdateSampleEntity1DapperRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class UpdateSampleEntity1DapperHandler(ICommandDbContext commandDbContext,
    ILogger<UpdateSampleEntity1DapperHandler> logger) : IMediatRCommandHandler<UpdateSampleEntity1DapperRequest, int>
{
    public Task<int> Handle(
        UpdateSampleEntity1DapperRequest request,
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
            logger.LogError(ex, "Error updating SampleEntity1Dapper.");
        }
        return Task.FromResult(0);
    }
}
