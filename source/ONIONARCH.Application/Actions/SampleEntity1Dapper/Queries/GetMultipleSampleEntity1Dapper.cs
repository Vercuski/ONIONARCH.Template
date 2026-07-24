using Dapper;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Dapper.Queries;

public sealed class GetMultipleSampleEntity1DappersRequest : IMediatRQueryRequest<List<SampleEntityDefinition>?>;
internal sealed class GetMultipleSampleEntity1DappersHandler(IDbReadOnlyConnectionFactory connectionFactory) : IMediatRQueryHandler<GetMultipleSampleEntity1DappersRequest, List<SampleEntityDefinition>?>
{
    public async Task<List<SampleEntityDefinition>?> Handle(
        GetMultipleSampleEntity1DappersRequest request,
        CancellationToken cancellationToken)
    {
        var sql = "SELECT * FROM table1";
        using var connection = connectionFactory.CreateConnection();
        var response = (await connection.QueryAsync<SampleEntityDefinition>(sql)).ToList();
        return response is null ? [] : response;
    }
}
