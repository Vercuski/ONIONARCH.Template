using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1EFCore.Queries;

public sealed class GetMultipleSampleEntity1EFCoresRequest : IMediatRQueryRequest<List<SampleEntityDefinition>>;
internal sealed class GetMultipleSampleEntity1EFCoresHandler(IQueryDbContext queryDbContext) : IMediatRQueryHandler<GetMultipleSampleEntity1EFCoresRequest, List<SampleEntityDefinition>>
{
    public async Task<List<SampleEntityDefinition>> Handle(
        GetMultipleSampleEntity1EFCoresRequest request,
        CancellationToken cancellationToken)
    {
        List<SampleEntityDefinition>? response = await queryDbContext.Set<SampleEntityDefinition>().ToListAsync(cancellationToken);

        if (response is null)
        {
            return await Task.FromResult(new List<SampleEntityDefinition>());
        }

        return response;
    }
}
