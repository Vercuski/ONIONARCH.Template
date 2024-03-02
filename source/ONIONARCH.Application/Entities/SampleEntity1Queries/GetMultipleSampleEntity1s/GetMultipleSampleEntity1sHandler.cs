using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.SampleEntity1Queries.GetMultipleSampleEntity1;

internal sealed class GetMultipleSampleEntity1sHandler(IQueryDbContext dbContext) : IQueryHandler<GetMultipleSampleEntity1sRequest, List<SampleEntity1>>
{
    private readonly IQueryDbContext _dbContext = dbContext;

    public Task<List<SampleEntity1>> Handle(
        GetMultipleSampleEntity1sRequest request,
        CancellationToken cancellationToken)
    {
        List<SampleEntity1>? response =
        [
            .. (
                from sampleEntity in _dbContext.Set<SampleEntity1>().AsNoTracking()
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
