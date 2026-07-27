using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Repositories;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

public sealed record DeleteSampleEntityDapperRequest(int SampleId)
    : IMediatRCommandRequest<Result<int>>;
internal sealed class DeleteSampleEntityDapperHandler(ISampleEntityDapperCommandRepository repository,
    ILogger<DeleteSampleEntityDapperHandler> logger)
    : IMediatRCommandHandler<DeleteSampleEntityDapperRequest, Result<int>>
{
    public async Task<Result<int>> Handle(
        DeleteSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var rowsAffected = await repository.DeleteAsync(request.SampleId, cancellationToken);
            return Result<int>.Success(rowsAffected);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SampleEntityDapper.");
            return Result<int>.Failure("Error deleting SampleEntityDapper.", ResultErrorType.Validation);
        }
    }
}
