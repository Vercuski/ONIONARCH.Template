using Dapper;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Queries;

public sealed class GetMultipleSampleEntityDappersRequest : IMediatRQueryRequest<List<SampleEntityDefinition>?>;
internal sealed class GetMultipleSampleEntityDappersHandler(IDbReadOnlyConnectionFactory connectionFactory) : IMediatRQueryHandler<GetMultipleSampleEntityDappersRequest, List<SampleEntityDefinition>?>
{
    public async Task<List<SampleEntityDefinition>?> Handle(
        GetMultipleSampleEntityDappersRequest request,
        CancellationToken cancellationToken)
    {
        var sql = "SELECT SampleId, SampleString, SampleBoolean, SampleInt, SampleDecimal FROM SampleTable";
        using var connection = connectionFactory.CreateConnection();
        var response = (await connection.QueryAsync<SampleEntityDefinition>(sql)).ToList();
        return response is null ? [] : response;
    }
}
