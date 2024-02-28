using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.Entity1.Queries.GetSingleEntity1;

internal sealed class GetSingleEntity1Handler(IQueryDbContext dbContext) : IQueryHandler<GetSingleEntity1Request, SampleEntity>
{
    private readonly IQueryDbContext _dbContext = dbContext;

    public Task<SampleEntity> Handle(
        GetSingleEntity1Request request,
        CancellationToken cancellationToken)
    {
        SampleEntity? response =
            (
                from sampleEntity in _dbContext.Set<SampleEntity>().AsNoTracking()
                select new SampleEntity
                {
                    SampleBoolean = false,
                    SampleDecimal = 0,
                    SampleId = 1,
                    SampleInt = 2,
                    SampleString = "string"
                }).SingleOrDefaultAsync(cancellationToken).Result;

        if (response is null)
        {
            return Task.FromResult(new SampleEntity());
        }

        return Task.FromResult(response);
    }
}
