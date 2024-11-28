using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1Queries.GetSingleSampleEntity1;
public sealed record GetSingleSampleEntity1Request(int Id) : IMediatRQueryRequest<SampleEntity1>;
internal sealed class GetSingleSampleEntity1Handler(
    IQueryDbContext queryDbContext
    ) : IMediatRQueryHandler<GetSingleSampleEntity1Request, SampleEntity1>
{
    public Task<SampleEntity1> Handle(
        GetSingleSampleEntity1Request request,
        CancellationToken cancellationToken)
    {
        SampleEntity1? response =
            (
                from sampleEntity in queryDbContext.Set<SampleEntity1>()
                    .AsNoTracking()
                select new SampleEntity1
                {
                    SampleBoolean1 = false,
                    SampleDecimal1 = 0,
                    SampleId1 = 1,
                    SampleInt1 = 2,
                    SampleString1 = "string"
                }).SingleOrDefaultAsync(cancellationToken).Result;

        if (response is null)
        {
            return Task.FromResult(new SampleEntity1());
        }

        return Task.FromResult(response);
    }
}
