using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Contracts.Dtos;

public sealed record SampleDtoRecord(int Id, string? SomeValue)
{
    public static SampleDtoRecord Create(SampleEntity1 entity1, SampleEntity2 entity2)
    {
        return new SampleDtoRecord(entity1.SampleId1, entity2.SampleString2);
    }
}
