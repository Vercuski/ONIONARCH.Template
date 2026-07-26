using Dapper;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Queries;

public sealed record GetSingleSampleEntityDapperRequest(int Id) : IMediatRQueryRequest<SampleEntityDefinition?>;
internal sealed class GetSingleSampleEntityDapperHandler(
    IDbReadOnlyConnectionFactory connectionFactory
    ) : IMediatRQueryHandler<GetSingleSampleEntityDapperRequest, SampleEntityDefinition?>
{
    public async Task<SampleEntityDefinition?> Handle(
        GetSingleSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        var sql = "SELECT SampleId, SampleString, SampleBoolean, SampleInt, SampleDecimal FROM SampleTable WHERE SampleId = @Id";
        using var connection = connectionFactory.CreateConnection();
        var response = await connection.QuerySingleOrDefaultAsync<SampleEntityDefinition>(sql, new { request.Id });
        return response;
    }
}
