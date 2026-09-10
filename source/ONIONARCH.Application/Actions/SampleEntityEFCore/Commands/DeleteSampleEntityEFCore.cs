using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

public sealed record DeleteSampleEntityEFCoreRequest(SampleEntityDefinition Entity)
    : ICommandRequest<Result<int>>;
internal sealed class DeleteSampleEntityEFCoreHandler(ICommandDbContext commandDbContext)
    : ICommandHandler<DeleteSampleEntityEFCoreRequest, Result<int>>
{
    public Task<Result<int>> Handle(
        DeleteSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        commandDbContext.Delete(request.Entity);
        int rowsAffected = commandDbContext.SaveChanges();
        return Task.FromResult(Result<int>.Success(rowsAffected));
    }
}
