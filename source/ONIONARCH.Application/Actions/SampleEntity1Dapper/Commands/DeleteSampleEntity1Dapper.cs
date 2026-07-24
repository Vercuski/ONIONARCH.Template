using Dapper;
using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Dapper.Commands;

public sealed record DeleteSampleEntity1DapperRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class DeleteSampleEntity1DapperHandler(IDbWriteConnectionFactory connectionFactory,
    ILogger<DeleteSampleEntity1DapperHandler> logger) : IMediatRCommandHandler<DeleteSampleEntity1DapperRequest, int>
{
    public async Task<int> Handle(
        DeleteSampleEntity1DapperRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected = 0;
        try
        {
            var sql = "DELETE FROM table1 WHERE value1=@value1";
            using var connection = connectionFactory.CreateConnection();
            rowsAffected = await connection.ExecuteAsync(sql, request.SampleEntity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SampleEntity1Dapper.");
        }
        return rowsAffected;
    }
}
