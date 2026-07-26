using Dapper;
using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

public sealed record UpdateSampleEntityDapperRequest(SampleEntityDefinition SampleEntity)
    : IMediatRCommandRequest<Result<int>>;
internal sealed class UpdateSampleEntityDapperHandler(IDbWriteConnectionFactory connectionFactory,
    ILogger<UpdateSampleEntityDapperHandler> logger)
    : IMediatRCommandHandler<UpdateSampleEntityDapperRequest, Result<int>>
{
    public async Task<Result<int>> Handle(
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
            return Result<int>.Failure("Error updating SampleEntityDapper.", ResultErrorType.Validation);
        }
        return Result<int>.Success(rowsAffected);
    }
}
