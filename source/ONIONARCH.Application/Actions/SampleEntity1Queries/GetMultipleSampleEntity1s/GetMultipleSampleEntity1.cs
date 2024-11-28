using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Queries.GetMultipleSampleEntity1s;

public sealed class GetMultipleSampleEntity1sRequest : IMediatRQueryRequest<List<SampleEntity1>>;
internal sealed class GetMultipleSampleEntity1sHandler(IQueryDbContext queryDbContext) : IMediatRQueryHandler<GetMultipleSampleEntity1sRequest, List<SampleEntity1>>
{
    public Task<List<SampleEntity1>> Handle(
        GetMultipleSampleEntity1sRequest request,
        CancellationToken cancellationToken)
    {
        List<SampleEntity1>? response =
        [
            .. (
                from sampleEntity in queryDbContext.Set<SampleEntity1>()
                    .AsNoTracking()
                select new SampleEntity1
                {
                    SampleBoolean1 = false,
                    SampleDecimal1 = 0,
                    SampleId1 = 1,
                    SampleInt1 = 2,
                    SampleString1 = "string"
                }),
        ];

        if (response is null)
        {
            return Task.FromResult(new List<SampleEntity1>());
        }

        return Task.FromResult(response);
    }
}
