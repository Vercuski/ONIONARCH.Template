using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.SampleEntity1Queries.GetMultipleSampleEntity1;

public sealed class GetMultipleSampleEntity1sRequest : IQuery<List<SampleEntity1>>
{
    public GetMultipleSampleEntity1sRequest()
    {
    }
}
