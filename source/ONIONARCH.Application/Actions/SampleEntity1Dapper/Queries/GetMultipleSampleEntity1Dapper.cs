using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Dapper.Queries;

public sealed class GetMultipleSampleEntity1DappersRequest : IMediatRQueryRequest<List<SampleEntityDefinition>>;
internal sealed class GetMultipleSampleEntity1DappersHandler(IQueryDbContext queryDbContext) : IMediatRQueryHandler<GetMultipleSampleEntity1DappersRequest, List<SampleEntityDefinition>>
{
    public Task<List<SampleEntityDefinition>> Handle(
        GetMultipleSampleEntity1DappersRequest request,
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
