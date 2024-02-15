using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.Entity1.Queries.GetSingleEntity1;

public sealed class GetSingleEntity1Request(int id) : IQuery<SampleEntity>
{
    public int Id { get; init; } = id;
}
