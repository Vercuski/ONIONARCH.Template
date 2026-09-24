using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Contracts.Dtos;

/// <summary>
/// Inbound payload for updating an existing sample entity.
/// </summary>
/// <param name="DtoSampleId">The key of the entity to update.</param>
/// <param name="DtoSampleString">The new value for <see cref="SampleEntityDefinition.SampleString"/>.</param>
/// <param name="DtoSampleBoolean">The new value for <see cref="SampleEntityDefinition.SampleBoolean"/>.</param>
/// <param name="DtoSampleInt">The new value for <see cref="SampleEntityDefinition.SampleInt"/>.</param>
/// <param name="DtoSampleDecimal">The new value for <see cref="SampleEntityDefinition.SampleDecimal"/>.</param>
public sealed record UpdateSampleRequestDto(
    int DtoSampleId,
    string? DtoSampleString,
    bool DtoSampleBoolean,
    int DtoSampleInt,
    decimal DtoSampleDecimal
    ) : IDomainMapper<SampleEntityDefinition>
{
    /// <summary>
    /// Creates a <see cref="SampleEntityDefinition"/> carrying this payload's key and values.
    /// </summary>
    /// <returns>A new, detached sample entity ready to be passed to an update command.</returns>
    public SampleEntityDefinition MapToDomain()
    {
        return new SampleEntityDefinition
        {
            SampleId = DtoSampleId,
            SampleString = DtoSampleString,
            SampleBoolean = DtoSampleBoolean,
            SampleInt = DtoSampleInt,
            SampleDecimal = DtoSampleDecimal
        };
    }
}