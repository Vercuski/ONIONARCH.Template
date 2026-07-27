using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Repositories;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

public sealed record UpdateSampleEntityDapperRequest(SampleEntityDefinition SampleEntity)
    : IMediatRCommandRequest<Result<int>>;
internal sealed class UpdateSampleEntityDapperHandler(ISampleEntityDapperCommandRepository repository,
    ILogger<UpdateSampleEntityDapperHandler> logger)
    : IMediatRCommandHandler<UpdateSampleEntityDapperRequest, Result<int>>
{
    public async Task<Result<int>> Handle(
        UpdateSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var rowsAffected = await repository.UpdateAsync(request.SampleEntity, cancellationToken);
            return Result<int>.Success(rowsAffected);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating SampleEntityDapper.");
            return Result<int>.Failure("Error updating SampleEntityDapper.", ResultErrorType.Validation);
        }
    }
}
