using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1EFCore.Queries;
public sealed record GetSingleSampleEntity1EFCoreRequest(int Id) : IMediatRQueryRequest<SampleEntityDefinition>;
internal sealed class GetSingleSampleEntity1EFCoreHandler(
    IQueryDbContext queryDbContext
    ) : IMediatRQueryHandler<GetSingleSampleEntity1EFCoreRequest, SampleEntityDefinition>
{
    public Task<SampleEntityDefinition> Handle(
        GetSingleSampleEntity1EFCoreRequest request,
        CancellationToken cancellationToken)
    {
        SampleEntityDefinition? response =
            (
                from sampleEntity in queryDbContext.Set<SampleEntityDefinition>()
                    .AsNoTracking()
                select new SampleEntityDefinition
                {
                    SampleBoolean1 = sampleEntity.SampleBoolean1,
                    SampleDecimal1 = sampleEntity.SampleDecimal1,
                    SampleId1 = sampleEntity.SampleId1,
                    SampleInt1 = sampleEntity.SampleInt1,
                    SampleString1 = sampleEntity.SampleString1
                }).SingleOrDefaultAsync(cancellationToken).Result;

        if (response is null)
        {
            return Task.FromResult(new SampleEntityDefinition());
        }

        return Task.FromResult(response);
    }
}
