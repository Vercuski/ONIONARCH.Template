using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

public sealed record CreateSampleEntityEFCoreRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class CreateSampleEntityEFCoreHandler(ICommandDbContext commandDbContext,
    ILogger<CreateSampleEntityEFCoreHandler> logger) : IMediatRCommandHandler<CreateSampleEntityEFCoreRequest, int>
{
    public Task<int> Handle(
        CreateSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected = 0;
        try
        {
            commandDbContext.Insert(request.SampleEntity);
            rowsAffected = commandDbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating SampleEntityEFCore.");
        }
        return Task.FromResult(rowsAffected);
    }
}
