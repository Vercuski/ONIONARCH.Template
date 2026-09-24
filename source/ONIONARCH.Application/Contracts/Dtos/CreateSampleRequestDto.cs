using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Contracts.Dtos;

/// <summary>
/// Inbound payload for creating a sample entity.
/// </summary>
/// <param name="DtoSampleString">The value for <see cref="SampleEntityDefinition.SampleString"/>.</param>
/// <param name="DtoSampleBoolean">The value for <see cref="SampleEntityDefinition.SampleBoolean"/>.</param>
/// <param name="DtoSampleInt">The value for <see cref="SampleEntityDefinition.SampleInt"/>.</param>
/// <param name="DtoSampleDecimal">The value for <see cref="SampleEntityDefinition.SampleDecimal"/>.</param>
public sealed record CreateSampleRequestDto(
    string? DtoSampleString,
    bool DtoSampleBoolean,
    int DtoSampleInt,
    decimal DtoSampleDecimal
    ) : IDomainMapper<SampleEntityDefinition>
{
    /// <summary>
    /// Creates a new <see cref="SampleEntityDefinition"/> from this payload.
    /// <see cref="SampleEntityDefinition.SampleId"/> is left at its default value.
    /// </summary>
    /// <returns>A new, unsaved sample entity.</returns>
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