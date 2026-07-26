using Dapper;
using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Queries;

public sealed record GetSingleSampleEntityDapperRequest(int Id)
    : IMediatRQueryRequest<Result<SampleEntityDefinition?>>;
internal sealed class GetSingleSampleEntityDapperHandler(
    IDbReadOnlyConnectionFactory connectionFactory,
    ILogger<GetMultipleSampleEntityDappersHandler> logger
    ) : IMediatRQueryHandler<GetSingleSampleEntityDapperRequest, Result<SampleEntityDefinition?>>
{
    public async Task<Result<SampleEntityDefinition?>> Handle(
        GetSingleSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        var sql = "SELECT SampleId, SampleString, SampleBoolean, SampleInt, SampleDecimal FROM SampleTable WHERE SampleId = @Id";
        using var connection = connectionFactory.CreateConnection();
        try
        {
            var response = await connection.QuerySingleOrDefaultAsync<SampleEntityDefinition>(sql, new { request.Id });
            return Result<SampleEntityDefinition?>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving SampleEntityDapper with Id {Id}.", request.Id);
            return Result<SampleEntityDefinition?>.Failure("Error retrieving SampleEntityDapper.", ResultErrorType.Validation);
        }
    }
}
