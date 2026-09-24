using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;

/// <summary>
/// Command to insert a new sample entity through the EF Core persistence path.
/// </summary>
/// <param name="SampleEntity">The entity to insert.</param>
public sealed record CreateSampleEntityEFCoreRequest(SampleEntityDefinition SampleEntity)
    : ICommandRequest<Result<int>>;

/// <summary>
/// Handles <see cref="CreateSampleEntityEFCoreRequest"/> using the EF Core command context.
/// </summary>
/// <param name="commandDbContext">The write-side EF Core context port.</param>
internal sealed class CreateSampleEntityEFCoreHandler(ICommandDbContext commandDbContext)
    : ICommandHandler<CreateSampleEntityEFCoreRequest, Result<int>>
{
    /// <summary>
    /// Stages the requested entity for insertion and saves it immediately.
    /// </summary>
    /// <param name="request">The command carrying the entity to insert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A successful result containing the number of state entries written.</returns>
    public async Task<Result<int>> Handle(
        CreateSampleEntityEFCoreRequest request,
        CancellationToken cancellationToken)
    {
        commandDbContext.Insert(request.SampleEntity);
        int rowsAffected = commandDbContext.SaveChanges();
        return Result<int>.Success(rowsAffected);
    }
}
