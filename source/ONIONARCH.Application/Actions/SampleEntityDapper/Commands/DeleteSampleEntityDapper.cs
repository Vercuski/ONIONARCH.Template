using Dapper;
using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

public sealed record DeleteSampleEntityDapperRequest(int SampleId) : IMediatRCommandRequest<int>;
internal sealed class DeleteSampleEntityDapperHandler(IDbWriteConnectionFactory connectionFactory,
    ILogger<DeleteSampleEntityDapperHandler> logger) : IMediatRCommandHandler<DeleteSampleEntityDapperRequest, int>
{
    public async Task<int> Handle(
        DeleteSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected = 0;
        try
        {
            var sql = "DELETE FROM SampleTable WHERE SampleId=@SampleId";
            using var connection = connectionFactory.CreateConnection();
            rowsAffected = await connection.ExecuteAsync(sql, new { request.SampleId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SampleEntityDapper.");
        }
        return rowsAffected;
    }
}
