using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1EFCore.Commands;
public sealed record CreateSampleEntity1EFCoreRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class CreateSampleEntity1EFCoreHandler(ICommandDbContext commandDbContext,
    ILogger<CreateSampleEntity1EFCoreHandler> logger) : IMediatRCommandHandler<CreateSampleEntity1EFCoreRequest, int>
{
    public Task<int> Handle(
        CreateSampleEntity1EFCoreRequest request,
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
            logger.LogError(ex, "Error creating SampleEntity1EFCore.");
        }
        return Task.FromResult(0);
    }
}
