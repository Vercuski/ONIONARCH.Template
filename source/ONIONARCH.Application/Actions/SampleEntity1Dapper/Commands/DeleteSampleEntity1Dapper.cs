using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Dapper.Commands;
public sealed record DeleteSampleEntity1DapperRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class DeleteSampleEntity1DapperHandler(IDbConnectionFactory connectionFactory,
    ILogger<DeleteSampleEntity1DapperHandler> logger) : IMediatRCommandHandler<DeleteSampleEntity1DapperRequest, int>
{
    public async Task<int> Handle(
        DeleteSampleEntity1DapperRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var sql = "DELETE FROM table1 WHERE value1=@value1";
            using var connection = connectionFactory.CreateWriteConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, request.SampleEntity);
            return rowsAffected;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SampleEntity1Dapper.");
        }
        return 0;
    }
}
