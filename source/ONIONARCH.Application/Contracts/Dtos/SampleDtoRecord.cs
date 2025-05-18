using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Contracts.Dtos;

public sealed record SampleDtoRecord(int Id)
{
    public static SampleDtoRecord Create(SampleEntityDefinition entity1)
    {
        return new SampleDtoRecord(entity1.SampleId1);
    }
}
