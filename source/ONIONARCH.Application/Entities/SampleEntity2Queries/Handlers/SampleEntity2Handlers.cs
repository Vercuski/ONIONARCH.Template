using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Entities.SampleEntity2Queries.Requests;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.SampleEntity2Queries.Handlers;

internal sealed class GetMultipleSampleEntity2sHandler(IQueryDbContext dbContext) : IQueryHandler<GetMultipleSampleEntity2sRequest, List<SampleEntity2>>
{
    private readonly IQueryDbContext _dbContext = dbContext;

    public Task<List<SampleEntity2>> Handle(
        GetMultipleSampleEntity2sRequest request,
        CancellationToken cancellationToken)
    {
        List<SampleEntity2>? response =
        [
            .. (
                from sampleEntity in _dbContext.Set<SampleEntity2>().AsNoTracking()
                select new SampleEntity2
                {
                    SampleBoolean2 = false,
                    SampleDecimal2 = 0,
                    SampleId2 = 1,
                    SampleInt2 = 2,
                    SampleString2 = "string"
                }),
        ];

        if (response is null)
        {
            return Task.FromResult(new List<SampleEntity2>());
        }

        return Task.FromResult(response);
    }
}

internal sealed class GetSingleSampleEntity2Handler(IQueryDbContext dbContext) : IQueryHandler<GetSingleSampleEntity2Request, SampleEntity2>
{
    private readonly IQueryDbContext _dbContext = dbContext;

    public Task<SampleEntity2> Handle(
        GetSingleSampleEntity2Request request,
        CancellationToken cancellationToken)
    {
        SampleEntity2? response =
            (
                from sampleEntity in _dbContext.Set<SampleEntity2>().AsNoTracking()
                select new SampleEntity2
                {
                    SampleBoolean2 = false,
                    SampleDecimal2 = 0,
                    SampleId2 = 1,
                    SampleInt2 = 2,
                    SampleString2 = "string"
                }).SingleOrDefaultAsync(cancellationToken).Result;

        if (response is null)
        {
            return Task.FromResult(new SampleEntity2());
        }

        return Task.FromResult(response);
    }
}
