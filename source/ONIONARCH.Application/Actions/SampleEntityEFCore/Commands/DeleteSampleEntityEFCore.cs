using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

/// <summary>
/// Command to delete a sample entity through the EF Core persistence path.
/// </summary>
/// <param name="Entity">
/// The entity to delete. The caller is expected to have loaded it first (the API does so via
/// <c>GetSingleSampleEntityEFCoreRequest</c>) so a missing entity is reported as not-found.
/// </param>
public sealed record DeleteSampleEntityEFCoreRequest(SampleEntityDefinition Entity)
    : ICommandRequest<Result<int>>;

/// <summary>
/// Handles <see cref="DeleteSampleEntityEFCoreRequest"/> using the EF Core command context.
/// </summary>
/// <param name="commandDbContext">The write-side EF Core context port.</param>
internal sealed class DeleteSampleEntityEFCoreHandler(ICommandDbContext commandDbContext)
    : ICommandHandler<DeleteSampleEntityEFCoreRequest, Result<int>>
{
    /// <summary>
    /// Stages the requested entity for deletion and saves immediately.
    /// </summary>
    /// <param name="request">The command carrying the entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A successful result containing the number of state entries written.</returns>
    public Task<Result<int>> Handle(
        DeleteSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        commandDbContext.Delete(request.Entity);
        int rowsAffected = commandDbContext.SaveChanges();
        return Task.FromResult(Result<int>.Success(rowsAffected));
    }
}
