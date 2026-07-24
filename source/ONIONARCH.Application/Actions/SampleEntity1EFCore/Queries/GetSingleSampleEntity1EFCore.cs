using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1EFCore.Queries;

public sealed record GetSingleSampleEntity1EFCoreRequest(int Id) : IMediatRQueryRequest<SampleEntityDefinition>;
internal sealed class GetSingleSampleEntity1EFCoreHandler(
    IQueryDbContext queryDbContext
    ) : IMediatRQueryHandler<GetSingleSampleEntity1EFCoreRequest, SampleEntityDefinition>
{
    public async Task<SampleEntityDefinition> Handle(
        GetSingleSampleEntity1EFCoreRequest request,
        CancellationToken cancellationToken)
    {
        SampleEntityDefinition? response = await queryDbContext.Set<SampleEntityDefinition>().SingleOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            return await Task.FromResult(new SampleEntityDefinition());
        }

        return response;
    }
}
