using Dapper;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Application.Abstractions.Repositories;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Persistence.Repositories;

/// <summary>
/// Dapper implementation of <see cref="ISampleEntityDapperCommandRepository"/>, executing
/// parameterized SQL against the <c>SampleTable</c> table in the command database.
/// </summary>
/// <param name="connectionFactory">Creates connections to the command database.</param>
public sealed class SampleEntityDapperCommandRepository(IDbWriteConnectionFactory connectionFactory)
    : ISampleEntityDapperCommandRepository
{
    /// <inheritdoc />
    public async Task<int> CreateAsync(SampleEntityDefinition entity, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO SampleTable (SampleId, SampleString, SampleBoolean, SampleInt, SampleDecimal) " +
                            "VALUES (@SampleId, @SampleString, @SampleBoolean, @SampleInt, @SampleDecimal)";
        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, entity, cancellationToken: cancellationToken);
        return await connection.ExecuteAsync(command);
    }

    /// <inheritdoc />
    public async Task<int> UpdateAsync(SampleEntityDefinition entity, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE SampleTable SET SampleString = @SampleString, SampleBoolean = @SampleBoolean, " +
                            "SampleInt = @SampleInt, SampleDecimal = @SampleDecimal WHERE SampleId = @SampleId";
        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, entity, cancellationToken: cancellationToken);
        return await connection.ExecuteAsync(command);
    }

    /// <inheritdoc />
    public async Task<int> DeleteAsync(int sampleId, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM SampleTable WHERE SampleId = @SampleId";
        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { SampleId = sampleId }, cancellationToken: cancellationToken);
        return await connection.ExecuteAsync(command);
    }
}
