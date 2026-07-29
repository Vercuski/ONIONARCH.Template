using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Repositories;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Queries;

public sealed class GetMultipleSampleEntityDappersRequest
    : IMediatRQueryRequest<Result<List<SampleEntityDefinition>?>>;
internal sealed class GetMultipleSampleEntityDappersHandler(
    ISampleEntityDapperQueryRepository repository)
    : IMediatRQueryHandler<GetMultipleSampleEntityDappersRequest, Result<List<SampleEntityDefinition>?>>
{
    public async Task<Result<List<SampleEntityDefinition>?>> Handle(
        GetMultipleSampleEntityDappersRequest request,
        CancellationToken cancellationToken)
    {
        var response = await repository.GetAllAsync(cancellationToken);
        return Result<List<SampleEntityDefinition>?>.Success(response);
    }
}
