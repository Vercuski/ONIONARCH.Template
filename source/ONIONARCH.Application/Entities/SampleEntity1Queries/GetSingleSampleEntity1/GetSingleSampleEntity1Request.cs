using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.SampleEntity1Queries.GetSingleSampleEntity1;

public sealed class GetSingleSampleEntity1Request(int id) : IQuery<SampleEntity1>
{
    public int Id { get; init; } = id;
}
