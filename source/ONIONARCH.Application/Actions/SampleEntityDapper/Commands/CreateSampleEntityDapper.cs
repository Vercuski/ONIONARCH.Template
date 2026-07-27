using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Repositories;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

public sealed record CreateSampleEntityDapperRequest(SampleEntityDefinition SampleEntity)
    : IMediatRCommandRequest<Result<int>>;
internal sealed class CreateSampleEntityDapperHandler(ISampleEntityDapperCommandRepository repository,
    ILogger<CreateSampleEntityDapperHandler> logger)
    : IMediatRCommandHandler<CreateSampleEntityDapperRequest, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var rowsAffected = await repository.CreateAsync(request.SampleEntity, cancellationToken);
            return Result<int>.Success(rowsAffected);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating SampleEntityDapper.");
            return Result<int>.Failure("Error creating SampleEntityDapper.", ResultErrorType.Validation);
        }
    }
}
