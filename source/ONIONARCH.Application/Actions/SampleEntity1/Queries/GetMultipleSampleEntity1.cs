using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1.Queries;

public sealed class GetMultipleSampleEntity1sRequest : IMediatRQueryRequest<List<SampleEntityDefinition>>;
internal sealed class GetMultipleSampleEntity1sHandler(IQueryDbContext queryDbContext) : IMediatRQueryHandler<GetMultipleSampleEntity1sRequest, List<SampleEntityDefinition>>
{
    public Task<List<SampleEntityDefinition>> Handle(
        GetMultipleSampleEntity1sRequest request,
        CancellationToken cancellationToken)
    {
        List<SampleEntityDefinition>? response =
        [
            .. 
                from sampleEntity in queryDbContext.Set<SampleEntityDefinition>()
                    .AsNoTracking()
                select new SampleEntityDefinition
                {
                    SampleBoolean1 = false,
                    SampleDecimal1 = 0,
                    SampleId1 = 1,
                    SampleInt1 = 2,
                    SampleString1 = "string"
                },
        ];

        if (response is null)
        {
            return Task.FromResult(new List<SampleEntityDefinition>());
        }

        return Task.FromResult(response);
    }
}
