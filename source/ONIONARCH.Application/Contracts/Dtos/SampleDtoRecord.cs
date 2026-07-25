using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Contracts.Dtos;

public sealed record SampleDtoRecord(int Id)
{
    public int DtoSampleId { get; private set; }
    public string? DtoSampleString { get; private set; }
    public bool DtoSampleBoolean { get; private set; }
    public int DtoSampleInt { get; private set; }
    public decimal DtoSampleDecimal { get; private set; }

    public static SampleDtoRecord Create(SampleEntityDefinition entity)
    {
        return new SampleDtoRecord(entity.SampleId)
        {
            DtoSampleString = entity.SampleString,
            DtoSampleBoolean = entity.SampleBoolean,
            DtoSampleInt = entity.SampleInt,
            DtoSampleDecimal = entity.SampleDecimal,
        };
    }
}
