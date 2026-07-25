using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Contracts.Dtos;

public sealed record CreateSampleRequestDto(
    string? DtoSampleString,
    bool DtoSampleBoolean,
    int DtoSampleInt,
    decimal DtoSampleDecimal
    ) : IDomainMapper<SampleEntityDefinition>
{
    public SampleEntityDefinition MapToDomain()
    {
        return new SampleEntityDefinition
        {
            SampleString = DtoSampleString,
            SampleBoolean = DtoSampleBoolean,
            SampleInt = DtoSampleInt,
            SampleDecimal = DtoSampleDecimal
        };
    }
}