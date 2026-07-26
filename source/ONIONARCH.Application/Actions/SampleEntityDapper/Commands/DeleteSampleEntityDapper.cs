using Dapper;
using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;

namespace ONIONARCH.Application.Actions.SampleEntityDapper.Commands;

public sealed record DeleteSampleEntityDapperRequest(int SampleId)
    : IMediatRCommandRequest<Result<int>>;
internal sealed class DeleteSampleEntityDapperHandler(IDbWriteConnectionFactory connectionFactory,
    ILogger<DeleteSampleEntityDapperHandler> logger)
    : IMediatRCommandHandler<DeleteSampleEntityDapperRequest, Result<int>>
{
    public async Task<Result<int>> Handle(
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
            return Result<int>.Failure("Error deleting SampleEntityDapper.", ResultErrorType.Validation);
        }
        return Result<int>.Success(rowsAffected);
    }
}
