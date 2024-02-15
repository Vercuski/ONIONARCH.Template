using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ONIONARCH.Application.Entities.Entity1.Queries.GetMultipleEntity1s;

internal sealed class GetMultipleEntity1sHandler(IQueryDbContext dbContext) : IQueryHandler<GetMultipleEntity1sRequest, List<SampleEntity>>
{
    private readonly IQueryDbContext _dbContext = dbContext;

    public Task<List<SampleEntity>> Handle(
        GetMultipleEntity1sRequest request,
        CancellationToken cancellationToken)
    {
        List<SampleEntity>? response =
        [
            .. (
                from sampleEntity in _dbContext.Set<SampleEntity>().AsNoTracking()
                select new SampleEntity
                {
                    SampleBoolean = false,
                    SampleDecimal = 0,
                    SampleId = 1,
                    SampleInt = 2,
                    SampleString = "string"
                }),
        ];

        if (response is null)
        {
            return Task.FromResult(new List<SampleEntity>());
        }

        return Task.FromResult(response);
    }
}
