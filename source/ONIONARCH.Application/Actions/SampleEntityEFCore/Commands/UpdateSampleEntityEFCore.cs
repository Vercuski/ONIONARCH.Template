using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

/// <summary>
/// Command to update an existing sample entity through the EF Core persistence path.
/// </summary>
/// <param name="SampleEntity">The entity carrying the key and new values.</param>
public sealed record UpdateSampleEntityEFCoreRequest(SampleEntityDefinition SampleEntity)
    : ICommandRequest<Result<int>>;

/// <summary>
/// Handles <see cref="UpdateSampleEntityEFCoreRequest"/> using the EF Core command context.
/// </summary>
/// <param name="commandDbContext">The write-side EF Core context port.</param>
internal sealed class UpdateSampleEntityEFCoreHandler(ICommandDbContext commandDbContext)
    : ICommandHandler<UpdateSampleEntityEFCoreRequest, Result<int>>
{
    /// <summary>
    /// Stages the requested entity for update and saves immediately.
    /// </summary>
    /// <param name="request">The command carrying the entity to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A successful result containing the number of state entries written.</returns>
    public Task<Result<int>> Handle(
        UpdateSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        commandDbContext.Alter(request.SampleEntity);
        int rowsAffected = commandDbContext.SaveChanges();
        return Task.FromResult(Result<int>.Success(rowsAffected));
    }
}
