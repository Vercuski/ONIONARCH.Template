using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Dapper.Commands;
public sealed record CreateSampleEntity1DapperRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class CreateSampleEntity1DapperHandler(ICommandDbContext commandDbContext,
    ILogger<CreateSampleEntity1DapperHandler> logger) : IMediatRCommandHandler<CreateSampleEntity1DapperRequest, int>
{
    public Task<int> Handle(
        CreateSampleEntity1DapperRequest request,
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
            logger.LogError(ex, "Error creating SampleEntity1Dapper.");
        }
        return Task.FromResult(0);
    }
}
