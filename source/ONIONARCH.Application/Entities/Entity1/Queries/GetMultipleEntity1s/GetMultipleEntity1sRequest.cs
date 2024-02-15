using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.Entity1.Queries.GetMultipleEntity1s;

public sealed class GetMultipleEntity1sRequest : IQuery<List<SampleEntity>>
{
    public GetMultipleEntity1sRequest()
    {
    }
}
