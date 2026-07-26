using Dapper;
using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Application.Actions.SampleEntityDapper.Commands;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Queries;

public sealed class GetMultipleSampleEntityDappersRequest
    : IMediatRQueryRequest<Result<List<SampleEntityDefinition>?>>;
internal sealed class GetMultipleSampleEntityDappersHandler(
    IDbReadOnlyConnectionFactory connectionFactory,
    ILogger<GetMultipleSampleEntityDappersHandler> logger)
    : IMediatRQueryHandler<GetMultipleSampleEntityDappersRequest, Result<List<SampleEntityDefinition>?>>
{
    public async Task<Result<List<SampleEntityDefinition>?>> Handle(
        GetMultipleSampleEntityDappersRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var sql = "SELECT SampleId, SampleString, SampleBoolean, SampleInt, SampleDecimal FROM SampleTable";
            using var connection = connectionFactory.CreateConnection();
            var response = (await connection.QueryAsync<SampleEntityDefinition>(sql)).ToList();
            return Result<List<SampleEntityDefinition>?>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving SampleEntityDappers.");
            return Result<List<SampleEntityDefinition>?>.Failure("Error retrieving SampleEntityDappers.", ResultErrorType.Validation);
        }
    }
}
