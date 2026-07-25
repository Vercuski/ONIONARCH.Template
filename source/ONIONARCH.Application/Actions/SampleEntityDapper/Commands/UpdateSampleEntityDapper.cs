using Dapper;
using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

public sealed record UpdateSampleEntityDapperRequest(SampleEntityDefinition SampleEntity) : IMediatRCommandRequest<int>;
internal sealed class UpdateSampleEntityDapperHandler(IDbWriteConnectionFactory connectionFactory,
    ILogger<UpdateSampleEntityDapperHandler> logger) : IMediatRCommandHandler<UpdateSampleEntityDapperRequest, int>
{
    public async Task<int> Handle(
        UpdateSampleEntityDapperRequest request,
        CancellationToken cancellationToken)
    {
        int rowsAffected = 0;
        try
        {
            var sql = "UPDATE SampleTable SET SampleString = @SampleString, SampleBoolean = @SampleBoolean, SampleInt = @SampleInt, SampleDecimal = @SampleDecimal WHERE SampleId=@SampleId";
            using var connection = connectionFactory.CreateConnection();
            rowsAffected = await connection.ExecuteAsync(sql, request.SampleEntity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating SampleEntityDapper.");
        }
        return rowsAffected;
    }
}
