using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntityEFCore.Queries;

public sealed class GetMultipleSampleEntityEFCoresRequest : IMediatRQueryRequest<List<SampleEntityDefinition>>;
internal sealed class GetMultipleSampleEntityEFCoresHandler(IQueryDbContext queryDbContext) : IMediatRQueryHandler<GetMultipleSampleEntityEFCoresRequest, List<SampleEntityDefinition>>
{
    public async Task<List<SampleEntityDefinition>> Handle(
        GetMultipleSampleEntityEFCoresRequest request,
        CancellationToken cancellationToken)
    {
        List<SampleEntityDefinition>? response = await queryDbContext.Set<SampleEntityDefinition>().ToListAsync(cancellationToken);
        return response;
    }
}
