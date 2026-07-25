using ONIONARCH.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ONIONARCH.Application.Contracts.Dtos;

public sealed record SampleDtoRecord(int Id)
{
    public int SampleId1 { get; private set; }
    public string? SampleString1 { get; private set; }
    public bool SampleBoolean1 { get; private set; }
    public int SampleInt1 { get; private set; }
    public decimal SampleDecimal1 { get; private set; }

    public static SampleDtoRecord Create(SampleEntityDefinition entity1)
    {
        return new SampleDtoRecord(entity1.SampleId1)
        {
            SampleString1 = entity1.SampleString1,
            SampleBoolean1 = entity1.SampleBoolean1,
            SampleInt1 = entity1.SampleInt1,
            SampleDecimal1 = entity1.SampleDecimal1,
        };
    }
}
