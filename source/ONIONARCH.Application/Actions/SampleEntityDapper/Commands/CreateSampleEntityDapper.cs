using Dapper;
using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

public sealed record CreateSampleEntityDapperRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class CreateSampleEntityDapperHandler(IDbWriteConnectionFactory connectionFactory,
    ILogger<CreateSampleEntityDapperHandler> logger)
    : IMediatRCommandHandler<CreateSampleEntityDapperRequest, int>
{
    public async Task<int> Handle(
        CreateSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected = 0;
        try
        {
            var sql = "INSERT INTO SampleTable (SampleId, SampleString, SampleBoolean, SampleInt, SampleDecimal) VALUES (@SampleId, @SampleString, @SampleBoolean, @SampleInt, @SampleDecimal)";
            using var connection = connectionFactory.CreateConnection();
            rowsAffected = await connection.ExecuteAsync(sql, request.SampleEntity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating SampleEntityDapper.");
        }
        return rowsAffected;
    }
}
