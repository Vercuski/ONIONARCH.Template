using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.SampleEntity2Queries.Requests;

public sealed class GetMultipleSampleEntity2sRequest() : IQuery<List<SampleEntity2>>;

public sealed class GetSingleSampleEntity2Request(int id) : IQuery<SampleEntity2>
{
    public int Id { get; init; } = id;
}
