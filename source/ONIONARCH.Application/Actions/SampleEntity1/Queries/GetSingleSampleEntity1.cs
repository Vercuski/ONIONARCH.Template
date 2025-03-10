﻿using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Actions.SampleEntity1.Queries;
public sealed record GetSingleSampleEntity1Request(int Id) : IMediatRQueryRequest<SampleEntityDefinition>;
internal sealed class GetSingleSampleEntity1Handler(
    IQueryDbContext queryDbContext
    ) : IMediatRQueryHandler<GetSingleSampleEntity1Request, SampleEntityDefinition>
{
    public Task<SampleEntityDefinition> Handle(
        GetSingleSampleEntity1Request request,
        CancellationToken cancellationToken)
    {
        SampleEntityDefinition? response =
            (
                from sampleEntity in queryDbContext.Set<SampleEntityDefinition>()
                    .AsNoTracking()
                select new SampleEntityDefinition
                {
                    SampleBoolean1 = false,
                    SampleDecimal1 = 0,
                    SampleId1 = 1,
                    SampleInt1 = 2,
                    SampleString1 = "string"
                }).SingleOrDefaultAsync(cancellationToken).Result;

        if (response is null)
        {
            return Task.FromResult(new SampleEntityDefinition());
        }

        return Task.FromResult(response);
    }
}
