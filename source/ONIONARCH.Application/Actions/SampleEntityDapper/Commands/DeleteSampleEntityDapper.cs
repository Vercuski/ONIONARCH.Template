using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Repositories;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

public sealed record DeleteSampleEntityDapperRequest(int SampleId)
    : ICommandRequest<Result<int>>;
internal sealed class DeleteSampleEntityDapperHandler(ISampleEntityDapperCommandRepository repository)
    : ICommandHandler<DeleteSampleEntityDapperRequest, Result<int>>
{
    public async Task<Result<int>> Handle(
        DeleteSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        var rowsAffected = await repository.DeleteAsync(request.SampleId, cancellationToken);
        return Result<int>.Success(rowsAffected);
    }
}
