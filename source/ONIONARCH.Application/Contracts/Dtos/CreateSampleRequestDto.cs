using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Contracts.Dtos;

public sealed record CreateSampleRequestDto(
    string? SampleString1,
    bool SampleBoolean1,
    int SampleInt1,
    decimal SampleDecimal1
    ) : IDomainMapper<SampleEntityDefinition>
{
    public SampleEntityDefinition MapToDomain()
    {
        return new SampleEntityDefinition
        {
            SampleString1 = SampleString1,
            SampleBoolean1 = SampleBoolean1,
            SampleInt1 = SampleInt1,
            SampleDecimal1 = SampleDecimal1
        };
    }
}