using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.SampleEntity1Queries.GetSingleSampleEntity1;

internal sealed class GetSingleSampleEntity1Handler(IQueryDbContext dbContext) : IQueryHandler<GetSingleSampleEntity1Request, SampleEntity1>
{
    private readonly IQueryDbContext _dbContext = dbContext;

    public Task<SampleEntity1> Handle(
        GetSingleSampleEntity1Request request,
        CancellationToken cancellationToken)
    {
        //SampleEntity1? response =
        //    (
        //        from sampleEntity in _dbContext.Set<SampleEntity1>().AsNoTracking()
        //        select new SampleEntity1
        //        {
        //            SampleBoolean1 = false,
        //            SampleDecimal1 = 0,
        //            SampleId1 = 1,
        //            SampleInt1 = 2,
        //            SampleString1 = "string"
        //        }).SingleOrDefaultAsync(cancellationToken).Result;

        SampleEntity1? response = new()
        {
            SampleBoolean1 = false,
            SampleDecimal1 = request.Id,
            SampleId1 = request.Id,
            SampleInt1 = request.Id,
            SampleString1 = $"string-{request.Id}"
        };

        if (response is null)
        {
            return Task.FromResult(new SampleEntity1());
        }

        return Task.FromResult(response);
    }
}
