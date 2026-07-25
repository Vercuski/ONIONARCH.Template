using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Queries;

public sealed record GetSingleSampleEntityEFCoreRequest(int Id) : IMediatRQueryRequest<SampleEntityDefinition>;
internal sealed class GetSingleSampleEntityEFCoreHandler(
    IQueryDbContext queryDbContext
    ) : IMediatRQueryHandler<GetSingleSampleEntityEFCoreRequest, SampleEntityDefinition>
{
    public async Task<SampleEntityDefinition> Handle(
        GetSingleSampleEntityEFCoreRequest request,
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
