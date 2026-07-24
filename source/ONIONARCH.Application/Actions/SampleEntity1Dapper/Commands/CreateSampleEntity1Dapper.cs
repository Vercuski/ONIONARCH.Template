using Dapper;
using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Dapper.Commands;

public sealed record CreateSampleEntity1DapperRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class CreateSampleEntity1DapperHandler(IDbWriteConnectionFactory connectionFactory,
    ILogger<CreateSampleEntity1DapperHandler> logger)
    : IMediatRCommandHandler<CreateSampleEntity1DapperRequest, int>
{
    public async Task<int> Handle(
        CreateSampleEntity1DapperRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected = 0;
        try
        {
            var sql = "INSERT INTO table1 (value1, value2) VALUES (@value1, @value2)";
            using var connection = connectionFactory.CreateConnection();
            rowsAffected = await connection.ExecuteAsync(sql, request.SampleEntity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating SampleEntity1Dapper.");
        }
        return rowsAffected;
    }
}
